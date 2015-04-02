using Mandrake.Model;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Mandrake.Management;
using Mandrake.Model.Document;


namespace Mandrake.Service
{
    public delegate void OTMessageEventHandler(object sender, OTMessage message);

    [DataContract]
    public class OTMessage
	{
        [DataMember]
        public List<Operation> Content { get; set; }

        public OTMessage()
        {
            Content = new List<Operation>();
        }

        public OTMessage(List<Operation> content): this()
        {
            Content.AddRange(content);
        }

        public OTMessage(Operation operation): this()
        {
            Content.Add(operation);
        }
	}

    [DataContract]
    public class OTAck
    {
        [DataMember]
        public int ClientMessages { get; set; }
        [DataMember]
        public int ServerMessages { get; set; }

        public OTAck(int clientMessages, int serverMessages)
        {
            ClientMessages = clientMessages;
            ServerMessages = serverMessages;
        }
    }

    [ServiceContract(CallbackContract = typeof(IOTCallback))]
    public interface IOTAwareService
    {
        [OperationContract]
        IEnumerable<ClientMetaData> Register(ClientMetaData msg);
        [OperationContract]
        void Send(OTMessage message);
        [OperationContract]
        void Hello(string msg);
        [OperationContract]
        void SendChatMessage(ChatMessage msg);
        [OperationContract]
        IEnumerable<Operation> GetLog(DocumentMetaData document);
        [OperationContract]
        IEnumerable<string> GetDocuments();
        [OperationContract]
        bool CreateDocument(DocumentMetaData document);
        [OperationContract]
        bool OpenDocument(DocumentMetaData document);
        [OperationContract]
        object SynchronizeDocument(DocumentMetaData document);
        [OperationContract]
        bool UploadDocument(DocumentMetaData meta, object content);
    }

    public interface IOTCallback
    {
        [OperationContract]
        void Forward(OTMessage message);
        [OperationContract]
        void SendAck(OTAck ack);
        [OperationContract]
        void Echo(string msg);
        [OperationContract]
        void ForwardChatMessage(ChatMessage msg);
        [OperationContract]
        void RegisterClient(ClientMetaData meta);
        [OperationContract]
        void NotifyDocumentCreated(string name);
        [OperationContract]
        void NotifyDocumentOpened(string name);
    }
}
