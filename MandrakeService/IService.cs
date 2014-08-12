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
    [DataContract]
    public class OTMessage
	{
        [DataMember]
        public List<Operation> Content { get; set; }
        [DataMember]
        public int ClientMessages { get; private set;}
        [DataMember]
        public int ServerMessages { get; private set;}

        public OTMessage(int clientMessages, int serverMessages)
        {
            ClientMessages = clientMessages;
            ServerMessages = serverMessages;
        }

        public OTMessage(int clientMessages, int serverMessages, List<Operation> content): this(clientMessages, serverMessages)
        {
            this.Content = content;
        }

        public OTMessage(int clientMessages, int serverMessages, Operation o): this(clientMessages, serverMessages)
        {
            Content = new List<Operation>();
            Content.Add(o);
        }

	}    

    [ServiceContract(CallbackContract = typeof(IOTCallback))]
    public interface IOTAwareService
    {
        [OperationContract]
        void Register(Guid id);
        [OperationContract]
        void Send(OTMessage message);
        [OperationContract]
        void Hello(string msg);
    }

    public interface IOTCallback
    {
        [OperationContract]
        void Forward(OTMessage message);
        [OperationContract]
        void SendAck(OTMessage acknowledgement);
        [OperationContract]
        void Synchronize(object content);
        [OperationContract]
        void Echo(string msg);
    }
}
