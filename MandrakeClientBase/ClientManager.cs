using Mandrake.Client.Base.OTServiceReference;
using Mandrake.Management;
using Mandrake.Model;
using Mandrake.Model.Document;
using Mandrake.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceModelEx;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Mandrake.Client.Base
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant,
        UseSynchronizationContext = false)]
    public class ClientManager : OTManager, IOTAwareServiceCallback
    {
        protected int clientMessages;

        private List<Operation> outgoing = new List<Operation>();
        private bool acknowledged = true;

        public event OperationActionEventHandler OperationPerformed;
        public event ChatMessageEventHandler MessageArrived;
        public event ClientRegisteredEventHandler ClientRegistered;
        public event DocumentCreatedEventHandler DocumentCreated;

        public Guid Id { get; set; }
        public Mandrake.Client.Base.OTServiceReference.IOTAwareService Service { get; set; }
        public string Name { get; set; }
        public List<ClientMetaData> Clients { get; set; }


        public ClientManager(IOTAwareContext context, string name)
        {
            Name = name;
            Id = Guid.NewGuid();
            Clients = new List<ClientMetaData>();
            AddDocument(context);
        }

        public ClientManager(IOTAwareContext context): this(context, "") { }

        public void OnChange(object sender, DocumentEventArgs e)
        {
            Operation o = null;

            foreach (var manager in ManagerChain)
            {
                if ((o = manager.TryRecognize(sender, e.Args)) != null)
                {
                    this.clientMessages++;

                    o.OwnerId = Id;
                    o.ClientMessages = clientMessages;
                    o.ServerMessages = serverMessages;
                    o.DocumentName = e.DocumentName;

                    TrySend(o);
                }
            }
        }

        private void TrySend(Operation o)
        {
            if (acknowledged)
            {
                acknowledged = false;

                Task.Factory.StartNew(() => Service.SendAsync(new OTMessage(o)));
            }

            else outgoing.Add(o);
        }

        protected override void Execute(Operation o, IOTAwareContext operationContext)
        {
            foreach (var manager in ManagerChain)
            {
                if (manager.TryExecute(operationContext, o)) return;
            }
        }

        protected override void Transform(Operation o, IOTAwareContext operationContext)
        {
            Operation copy = (Operation)o.Clone();

            outgoing.Select(x =>
            {
                o = transformer.Transform(x, o);
                return transformer.Transform(copy, x);
            });
        }

        public void Forward(OTMessage message)
        {
            Task.Factory.StartNew(() => ProcessMessage(message));
        }

        private void ProcessMessage(OTMessage message)
        {
            foreach (var o in message.Content)
            {
                if (!Documents.ContainsKey(o.DocumentName)) continue;

                var operationContext = Documents[o.DocumentName];
                Transform(o, operationContext);
                Execute(o, operationContext);

                operationContext.ServerMessages++;

                o.ExecutedAt = DateTime.Now;
                o.ClientMessages = this.clientMessages;
                o.ServerMessages = this.serverMessages;

                operationContext.Log.Add(o);
            }
        }

        public void SendAck(OTAck ack)
        {
            if (outgoing.Count != 0)
            {
                Task.Factory.StartNew(() =>
                {
                    Service.SendAsync(new OTMessage(outgoing));
                    outgoing.Clear();
                });
            }

            else acknowledged = true;
        }

        public void Echo(string msg)
        {
            Console.WriteLine(this.Id + " got: " + msg);
        }

        public void Connect(string name)
        {
            var ic = new InstanceContext(this);
            var proxy = new OTAwareServiceClient(ic);
            proxy.AddGenericResolver();
            Service = proxy;
            Name = name;

            Clients.AddRange(proxy.Register(new ClientMetaData() { Id = this.Id, Name = name }).ToList());
            Clients.ForEach(c =>
            {
                if (ClientRegistered != null) ClientRegistered(this, c);
            });

            proxy.Hello("Hello Server!");
        }

        public void ForwardChatMessage(ChatMessage msg)
        {
            Messages.Add(msg);

            if (MessageArrived != null) MessageArrived(this, msg);
        }

        public ChatMessage SendChatMessage(string content)
        {
            var message = new ChatMessage(content, Name, Id);

            Messages.Add(message);
            Service.SendChatMessageAsync(message);

            return message;
        }


        public void RegisterClient(ClientMetaData meta)
        {
            Clients.Add(meta);
            
            if (ClientRegistered != null) ClientRegistered(this, meta);
        }

        public async Task<IEnumerable<Operation>> GetHistory(string documentName)
        {
            return await Service.GetLogAsync(new DocumentMetaData { Name = documentName, ClientId = this.Id });
        }

        public async Task<IOTAwareContext> CreateDocument(string name)
        {
            if ((await Service.CreateDocumentAsync(new DocumentMetaData { Name = name, ClientId = this.Id })))
            {
                return DocumentFactory.CreateDocument(name);
            }

            return null;
        }


        public void NotifyDocumentCreated(string name)
        {
            //Documents[name] = Context = DocumentFactory.CreateDocument(name);
            //Log.Clear();

            if (DocumentCreated != null) DocumentCreated(this, Documents[name]);
        }

        public void NotifyDocumentOpened(string name)
        {
            //Log.Clear();
            //syncManager.SetContent(Context, Service.SynchronizeDocumentAsync(new DocumentMetaData { Name = name, ClientId = this.Id }));
        }

        public async Task<IEnumerable<string>> GetAvailableDocuments()
        {
            return  await Service.GetDocumentsAsync();
        }

        public async Task<IOTAwareContext> OpenDocument(string name)
        {
            var meta = new DocumentMetaData { Name = name, ClientId = this.Id };

            if ((await Service.OpenDocumentAsync(meta)))
            {
                var openedDocument = DocumentFactory.CreateDocument(name);
                Documents[name] = openedDocument;
                syncManager.SetContent(openedDocument, await Service.SynchronizeDocumentAsync(meta));

                return openedDocument;
            }

            return null;
        }

        public async Task<IOTAwareContext> UploadDocument(string name, object content)
        {
            if (await Service.UploadDocumentAsync(new DocumentMetaData { Name = name, ClientId = this.Id }, content))
            {
                var newDocument = DocumentFactory.CreateDocument(name);
                syncManager.SetContent(newDocument, content);
                Documents[name] = newDocument;

                return newDocument;
            }

            return null;
        }
    }
}
