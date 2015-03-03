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

    public abstract class OTManager
    {
        protected int serverMessages;

        //[Import(typeof(ISynchronize))]
        protected ISynchronize syncManager;

        [Import(typeof(ITransform))]
        public ITransform transformer { get; set; }

        //[Import(typeof(IOTAwareContext))]
        public IOTAwareContext Context { get; set; }
        
        [ImportMany]
        public IEnumerable<IOperationManager> ManagerChain { get; set; }
        public List<Operation> Log { get; protected set; }
        public List<ChatMessage> Messages { get; set; }

        public OTManager()
        {
            Log = new List<Operation>();
            Messages = new List<ChatMessage>();
            var catalog = new AggregateCatalog();

            Array.ForEach(AppDomain.CurrentDomain.GetAssemblies(), x => catalog.Catalogs.Add(new AssemblyCatalog(x)));
            if (Assembly.GetEntryAssembly() == null)
            {
                Array.ForEach(GenericResolverInstaller.GetWebAssemblies(), x => catalog.Catalogs.Add(new AssemblyCatalog(x)));
            }

            var container = new CompositionContainer(catalog);

            try
            {
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        protected abstract void Transform(Operation o);
        protected abstract void Execute(Operation o);
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


    public interface IOperationManager
    {
        Operation TryRecognize(object sender, EventArgs e);
        bool TryExecute(object context, Operation o);
    }

    public interface ISynchronize
    {
        void Synchronize(object content);
    }
}