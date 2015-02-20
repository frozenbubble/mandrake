using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Mandrake.Management;
using System.Runtime.CompilerServices;
using Mandrake.Model;
using Mandrake.Model.Document;
using System.Threading;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Threading;
using System.Configuration;
using Mandrake.Service.Configuration;
using System.Reflection;

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

            var contextInfo = ConfigurationManager.GetSection("Mandrake/ContextMetaData") as ServiceContextConfiguration;
            if (contextInfo != null && contextInfo.Context.Assembly != string.Empty && contextInfo.Context.Type != string.Empty)
            {
                try
                {
                    var contextElement = contextInfo.Context;
                    Type contextType = Assembly.Load(contextElement.Assembly).GetType(contextElement.Type);

                    Context = (IOTAwareContext)Activator.CreateInstance(contextType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not instantiate context object" + Environment.NewLine + ex.Message);
                }
            }
        }

        public void Send(OTMessage message)
        {
            lock (syncroot)
            {
                if (MessageArrived != null) MessageArrived(this, message);

                var ops = message.Content;
                var cached = Log.Where(item => item.ServerMessages > ops.FirstOrDefault().ServerMessages);

                foreach (var op in ops)
                {
                    foreach (var cachedOp in cached)
                    {
                        transformer.Transform(cachedOp, op);
                    }

                    Execute(op);
                    this.serverMessages++;
                }

                Broadcast(message);
                SendAck(message);
            }
        }

        private void SendAck(OTMessage msg)
        {
            var to = Clients.Where(c => c.Key == msg.Content.FirstOrDefault().OwnerId).FirstOrDefault();
            to.Value.ClientMessages += msg.Content.Count;

            var ack = new OTAck(to.Value.ClientMessages, this.serverMessages);
            to.Value.Client.SendAck(ack);
        }

        private void Broadcast(OTMessage msg)
        {
            var to = Clients.Where(c => c.Key != msg.Content.FirstOrDefault().OwnerId);

            foreach (var entry in to)
            {
                entry.Value.Client.Forward(msg);

                if (MessageSent != null) MessageSent(this, msg);
            }
        }


        public void Register(Guid id)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IOTCallback>();
            Clients.Add(id, new SynchronizingConnection(callback));

            if (RegistrationCompleted != null) RegistrationCompleted(this, null);
        }

        protected override void Transform(Operation o)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(Operation o)
        {
            foreach (var manager in ManagerChain)
            {
                if (manager.TryExecute(Context, o))
                {
                    o.ExecutedAt = DateTime.Now;
                    o.ServerMessages = this.serverMessages;
                    Log.Add(o);

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
            chatMessages.Add(msg);

            if (ChatMessageArrived != null) ChatMessageArrived(this, msg);

            var to = Clients.Where(c => c.Key != msg.SenderId);
            
            foreach (var client in to) client.Value.Client.ForwardChatMessage(msg);

        }
    }

    public class SynchronizingConnection
    {
        public int ClientMessages { get; set; }
        public IOTCallback Client { get; set; }

        public SynchronizingConnection(IOTCallback cb)
        {
            Client = cb;
        }
    }
}
