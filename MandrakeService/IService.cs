using Mandrake.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Mandrake.Management;


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

        public OTMessage(List<Operation> content)
        {
            Content = content;
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
        IEnumerable<RegistrationMessage> Register(RegistrationMessage msg);
        [OperationContract]
        void Send(OTMessage message);
        [OperationContract]
        void Hello(string msg);
        [OperationContract]
        void SendChatMessage(ChatMessage msg);
    }

    public interface IOTCallback
    {
        [OperationContract]
        void Forward(OTMessage message);
        [OperationContract]
        void SendAck(OTAck ack);
        [OperationContract]
        void Synchronize(object content);
        [OperationContract]
        void Echo(string msg);
        [OperationContract]
        void ForwardChatMessage(ChatMessage msg);
        [OperationContract]
        void RegisterClient(Guid id);
    }
}
