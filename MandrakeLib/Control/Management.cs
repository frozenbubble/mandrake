using ICSharpCode.AvalonEdit.Document;
using Mandrake.Model;
using Mandrake.Model.Document;
using ServiceModelEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Runtime.Serialization;


namespace Mandrake.Management
{
    public delegate void OperationActionEventHandler(object sender, Operation o);
    public delegate void ChatMessageEventHandler(object sender, ChatMessage msg);
    public delegate void ClientRegisteredEventHandler(object sender, ClientMetaData meta);
    public delegate void DocumentCreatedEventHandler(object sender, IOTAwareContext document);

    public abstract class OTManager
    {
        protected int serverMessages;

        [Import(typeof(ISynchronize))]
        protected ISynchronize syncManager;
        [Import(typeof(IDocumentFactory))]
        public IDocumentFactory DocumentFactory { get; set; }
        [Import(typeof(ITransform))]
        public ITransform transformer { get; set; }
        [ImportMany]
        public IEnumerable<IOperationManager> ManagerChain { get; set; }

        public List<ChatMessage> Messages { get; set; }
        public Dictionary<string, IOTAwareContext> Documents { get; set; }

        public OTManager()
        {
            Messages = new List<ChatMessage>();
            Documents = new Dictionary<string, IOTAwareContext>();
            MandrakeBuilder.Compose(this);
        }

        public void AddDocument(IOTAwareContext document)
        {
            Documents[document.DocumentName] = document;
        }

        protected abstract void Transform(Operation o, IOTAwareContext operationContext);
        protected abstract void Execute(Operation o, IOTAwareContext operationContext);
    }

    public class DocumentEventArgs : EventArgs
    {
        public string DocumentName { get; set; }
        public EventArgs Args { get; set; }
    }

    [DataContract]
    public class ChatMessage
    {
        [DataMember]
        public Guid SenderId { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public string SenderName { get; private set; }
        [DataMember]
        public DateTime TimeStamp { get; private set; }

        public ChatMessage(string message, string sender, Guid id)
        {
            SenderId = id;
            Message = message;
            SenderName = sender;
            TimeStamp = DateTime.Now;
        }

        public override string ToString()
        {
            return String.Format("[{0}] {1}: {2}", TimeStamp, SenderName, Message);
        }

    }

    [DataContract]
    public class ClientMetaData
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [DataContract]
    public class DocumentMetaData
    {
        [DataMember]
        public Guid ClientId { get; set; }
        [DataMember]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }


    public interface IOperationManager
    {
        Operation TryRecognize(object sender, EventArgs e);
        bool TryExecute(object context, Operation o);
    }

    public class AggregateOperationManager : IOperationManager
    {
        [ImportMany]
        public IEnumerable<IOperationManager> ManagerChain { get; set; }

        public AggregateOperationManager()
        {
            MandrakeBuilder.Compose(this);
        }

        public Operation TryRecognize(object sender, EventArgs e)
        {
            return null;
        }

        public bool TryExecute(object context, Operation o)
        {
            var composite = o as AggregateOperation;

            if (composite == null) return false;

            return composite.Operations.TrueForAll(op =>
            {
                foreach (var manager in ManagerChain)
                {
                    if (manager.TryExecute(context, op)) return true;
                }

                return false;
            });
        }
    }


    public interface ISynchronize
    {
        object GetContent(IOTAwareContext ctx);
        void SetContent(IOTAwareContext ctx, object value);
    }

    internal static class MandrakeBuilder
    {

        internal static void Compose(object instance)
        {
            var catalog = new AggregateCatalog();

            Array.ForEach(AppDomain.CurrentDomain.GetAssemblies(), x => catalog.Catalogs.Add(new AssemblyCatalog(x)));
            if (Assembly.GetEntryAssembly() == null)
            {
                Array.ForEach(GenericResolverInstaller.GetWebAssemblies(), x => catalog.Catalogs.Add(new AssemblyCatalog(x)));
            }

            var container = new CompositionContainer(catalog);

            try
            {
                container.ComposeParts(instance);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }
    }
}