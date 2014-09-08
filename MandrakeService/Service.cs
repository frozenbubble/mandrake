using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Mandrake.Management;
using System.Runtime.CompilerServices;
using Mandrake.Model;
using Mandrake.View.Controls;
using System.Threading;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Threading;

namespace Mandrake.Service
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant,
                     InstanceContextMode = InstanceContextMode.Single)]
    public class OTAwareService : OTManager, IOTAwareService
    {
        private readonly object syncroot = new object();

        public Dictionary<Guid, SynchronizingConnection> Clients { get; set; }

        public event OTMessageEventHandler MessageArrived;
        public event OperationActionEventHandler OperationPerformed;
        public event OTMessageEventHandler MessageSent;
        public event OperationActionEventHandler RegistrationCompleted;

        public OTAwareService()
        {
            transformer = new LogTransformer();           // autofac

            // for now - autofac?
            ManagerChain = new List<IOperationManager>();
            ManagerChain.Add(new BasicInsertOperationManager());
            ManagerChain.Add(new BasicDeleteOperationManager());

            Log = new List<Operation>();
            //???????
            Context = new OTAwareDocument();
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

            if (Clients == null) Clients = new Dictionary<Guid, SynchronizingConnection>();

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

                    return;
                }
            }
        }


        public void Hello(string msg)
        {
            Console.WriteLine("server got hello");

            foreach (var connection in Clients)
            {
                connection.Value.Client.Echo("Hello from" + connection.Key);
            }
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
