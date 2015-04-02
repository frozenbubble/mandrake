using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Mandrake.Management;
using Mandrake.Model;
using Mandrake.Model.Document;
using System.Configuration;
using Mandrake.Service.Configuration;
using System.Reflection;
using System.Threading.Tasks;

namespace Mandrake.Service
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public class OTAwareService : OTManager, IOTAwareService
    {
        private readonly object syncroot = new object();

        public Dictionary<Guid, SynchronizingConnection> Clients { get; set; }

        public event OTMessageEventHandler MessageSent;
        public event OTMessageEventHandler MessageArrived;
        public event ChatMessageEventHandler ChatMessageArrived;
        public event ChatMessageEventHandler ChatMessageAcknowledged;
        public event OperationActionEventHandler OperationPerformed;
        public event OperationActionEventHandler RegistrationCompleted;

        public OTAwareService(): base() 
        {
            Clients = new Dictionary<Guid, SynchronizingConnection>();
        }

        public void Send(OTMessage message)
        {
            Task.Factory.StartNew(() => ProcessMessage(message));
        }

        private void ProcessMessage(OTMessage message)
        {
            if (message.Content.Count == 0) return;

            if (MessageArrived != null) MessageArrived(this, message);

            foreach (var op in message.Content)
            {
                var operationContext = Documents[op.DocumentName];
                var cachedOps = operationContext.Log.Where(item => item.ServerMessages > op.ServerMessages && !item.OwnerId.Equals(op.OwnerId));

                Operation transformed = null;
                
                foreach (var cachedOp in cachedOps) transformed = transformer.Transform(cachedOp, op);

                Execute(op, operationContext);
                operationContext.ServerMessages++;
            }

            Broadcast(message);
            SendAck(message);
        }

        private void SendAck(OTMessage msg)
        {
            var to = Clients.Where(c => c.Key == msg.Content.FirstOrDefault().OwnerId).FirstOrDefault();
            to.Value.ClientMessages += msg.Content.Count;

            var ack = new OTAck(to.Value.ClientMessages, this.serverMessages);
            Task.Factory.StartNew(() => to.Value.Client.SendAck(ack));
        }

        private void Broadcast(OTMessage msg)
        {
            var to = Clients.Where(c => c.Key != msg.Content.FirstOrDefault().OwnerId).ToList();

            foreach (var entry in to)
            {
                Task.Factory.StartNew(() => entry.Value.Client.Forward(msg));

                if (MessageSent != null) MessageSent(this, msg);
            }
        }


        public IEnumerable<ClientMetaData> Register(ClientMetaData msg)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IOTCallback>();
            Clients.Add(msg.Id, new SynchronizingConnection(callback, msg.Name));
            
            var others = Clients.Where(c => c.Key != msg.Id).ToList();
            others.ForEach(c => c.Value.Client.RegisterClient(msg));

            if (RegistrationCompleted != null) RegistrationCompleted(this, null);

            return others.Select(c => new ClientMetaData() { Id = c.Key, Name = c.Value.Name });
        }

        protected override void Transform(Operation o, IOTAwareContext operationContext)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(Operation o, IOTAwareContext operationContext)
        {
            foreach (var manager in ManagerChain)
            {
                if (manager.TryExecute(operationContext, o))
                {
                    o.ExecutedAt = DateTime.Now;
                    o.ServerMessages = this.serverMessages;
                    operationContext.Log.Add(o);

                    if (OperationPerformed != null) OperationPerformed(this, o);
                }
            }
        }


        public void Hello(string msg)
        {
            Console.WriteLine("server got hello");

            foreach (var connection in Clients) connection.Value.Client.Echo("Hello from" + connection.Key);
        }


        public void SendChatMessage(ChatMessage msg)
        {
            Messages.Add(msg);

            if (ChatMessageArrived != null) ChatMessageArrived(this, msg);

            var to = Clients.Where(c => c.Key != msg.SenderId);
            
            foreach (var client in to) client.Value.Client.ForwardChatMessage(msg);

        }

        public IEnumerable<Operation> GetLog(DocumentMetaData document)
        {
            return Documents[document.Name].Log;
        }


        public IEnumerable<string> GetDocuments()
        {
            return Documents.Keys;
        }

        public bool CreateDocument(DocumentMetaData document)
        {
            if (!Documents.ContainsKey(document.Name))
            {
                var newInstance = DocumentFactory.CreateDocument(document.Name);
                Documents[document.Name] = newInstance;

                var to = Clients.Where(c => c.Key != document.ClientId);
                Parallel.ForEach(to, c => c.Value.Client.NotifyDocumentCreated(document.Name));
                
                return true;
            }

            else return false;
        }

        public bool OpenDocument(DocumentMetaData document)
        {
            return Documents.ContainsKey(document.Name);
        }

        public object SynchronizeDocument(DocumentMetaData document)
        {
            return syncManager.GetContent(Documents[document.Name]);
        }


        public bool UploadDocument(DocumentMetaData meta, object content)
        {
            if (!Documents.ContainsKey(meta.Name))
            {
                var newDocument = DocumentFactory.CreateDocument(meta.Name);
                syncManager.SetContent(newDocument, content);
                Documents[meta.Name] = newDocument;

                return true;
            }

            return false;
        }
    }

    public class SynchronizingConnection
    {
        public int ClientMessages { get; set; }
        public IOTCallback Client { get; set; }
        public string Name { get; set; }

        public SynchronizingConnection(IOTCallback cb, string name)
        {
            Client = cb;
            Name = name;
        }
    }
}
