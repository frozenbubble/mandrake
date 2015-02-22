﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mandrake.Client.Base.OTServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="OTServiceReference.IOTAwareService", CallbackContract=typeof(Mandrake.Client.Base.OTServiceReference.IOTAwareServiceCallback))]
    public interface IOTAwareService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/Register", ReplyAction="http://tempuri.org/IOTAwareService/RegisterResponse")]
        Mandrake.Management.RegistrationMessage[] Register(Mandrake.Management.RegistrationMessage msg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/Register", ReplyAction="http://tempuri.org/IOTAwareService/RegisterResponse")]
        System.Threading.Tasks.Task<Mandrake.Management.RegistrationMessage[]> RegisterAsync(Mandrake.Management.RegistrationMessage msg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/Send", ReplyAction="http://tempuri.org/IOTAwareService/SendResponse")]
        void Send(Mandrake.Service.OTMessage message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/Send", ReplyAction="http://tempuri.org/IOTAwareService/SendResponse")]
        System.Threading.Tasks.Task SendAsync(Mandrake.Service.OTMessage message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/Hello", ReplyAction="http://tempuri.org/IOTAwareService/HelloResponse")]
        void Hello(string msg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/Hello", ReplyAction="http://tempuri.org/IOTAwareService/HelloResponse")]
        System.Threading.Tasks.Task HelloAsync(string msg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/SendChatMessage", ReplyAction="http://tempuri.org/IOTAwareService/SendChatMessageResponse")]
        void SendChatMessage(Mandrake.Management.ChatMessage msg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/SendChatMessage", ReplyAction="http://tempuri.org/IOTAwareService/SendChatMessageResponse")]
        System.Threading.Tasks.Task SendChatMessageAsync(Mandrake.Management.ChatMessage msg);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOTAwareServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/Forward", ReplyAction="http://tempuri.org/IOTAwareService/ForwardResponse")]
        void Forward(Mandrake.Service.OTMessage message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/SendAck", ReplyAction="http://tempuri.org/IOTAwareService/SendAckResponse")]
        void SendAck(Mandrake.Service.OTAck ack);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/Synchronize", ReplyAction="http://tempuri.org/IOTAwareService/SynchronizeResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Mandrake.Management.RegistrationMessage))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Mandrake.Management.RegistrationMessage[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Mandrake.Management.ChatMessage))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Mandrake.Service.OTMessage))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Mandrake.Service.OTAck))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Mandrake.Model.Operation[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Mandrake.Model.Operation))]
        void Synchronize(object content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/Echo", ReplyAction="http://tempuri.org/IOTAwareService/EchoResponse")]
        void Echo(string msg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/ForwardChatMessage", ReplyAction="http://tempuri.org/IOTAwareService/ForwardChatMessageResponse")]
        void ForwardChatMessage(Mandrake.Management.ChatMessage msg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOTAwareService/RegisterClient", ReplyAction="http://tempuri.org/IOTAwareService/RegisterClientResponse")]
        void RegisterClient(System.Guid id);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOTAwareServiceChannel : Mandrake.Client.Base.OTServiceReference.IOTAwareService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OTAwareServiceClient : System.ServiceModel.DuplexClientBase<Mandrake.Client.Base.OTServiceReference.IOTAwareService>, Mandrake.Client.Base.OTServiceReference.IOTAwareService {
        
        public OTAwareServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public OTAwareServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public OTAwareServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public OTAwareServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public OTAwareServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public Mandrake.Management.RegistrationMessage[] Register(Mandrake.Management.RegistrationMessage msg) {
            return base.Channel.Register(msg);
        }
        
        public System.Threading.Tasks.Task<Mandrake.Management.RegistrationMessage[]> RegisterAsync(Mandrake.Management.RegistrationMessage msg) {
            return base.Channel.RegisterAsync(msg);
        }
        
        public void Send(Mandrake.Service.OTMessage message) {
            base.Channel.Send(message);
        }
        
        public System.Threading.Tasks.Task SendAsync(Mandrake.Service.OTMessage message) {
            return base.Channel.SendAsync(message);
        }
        
        public void Hello(string msg) {
            base.Channel.Hello(msg);
        }
        
        public System.Threading.Tasks.Task HelloAsync(string msg) {
            return base.Channel.HelloAsync(msg);
        }
        
        public void SendChatMessage(Mandrake.Management.ChatMessage msg) {
            base.Channel.SendChatMessage(msg);
        }
        
        public System.Threading.Tasks.Task SendChatMessageAsync(Mandrake.Management.ChatMessage msg) {
            return base.Channel.SendChatMessageAsync(msg);
        }
    }
}
