﻿using Mandrake.Client.Base.OTServiceReference;
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

        public Guid Id { get; set; }
        public Mandrake.Client.Base.OTServiceReference.IOTAwareService Service { get; set; }

        public ClientManager(IOTAwareContext context): base()
        {
            Context = context;
            Id = Guid.NewGuid();
        }

        public void Synchronize(object content)
        {
            syncManager.Synchronize(content);
        }

        public void OnChange(object sender, EventArgs e)
        {
            if (!Context.IsUpdatedByUser) return;

            Operation o = null;

            foreach (var manager in ManagerChain)
            {
                if ((o = manager.TryRecognize(sender, e)) != null)
                {
                    this.clientMessages++;

                    o.OwnerId = Id;
                    o.ClientMessages = clientMessages;
                    o.ServerMessages = serverMessages;

                    TrySend(o);

                    //break;
                }
            }
        }

        private void TrySend(Operation o)
        {
            if (acknowledged)
            {
                acknowledged = false;

                //Parallel.Invoke(() => Service.Send(new OTMessage(o)));
                //Task.Factory.StartNew(() => Service.SendAsync(new OTMessage(o)));
                Service.SendAsync(new OTMessage(o));
            }

            else outgoing.Add(o);
        }

        protected override void Execute(Operation o)
        {
            foreach (var manager in ManagerChain)
            {
                if (manager.TryExecute(Context, o)) return;
            }
        }

        protected override void Transform(Operation o)
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
            //Task.Factory.StartNew(() => ProcessMessage(message));
            ProcessMessage(message);
        }

        private void ProcessMessage(OTMessage message)
        {
            foreach (var o in message.Content)
            {
                Transform(o);
                Execute(o);

                this.serverMessages++;
                o.ExecutedAt = DateTime.Now;
                o.ClientMessages = this.clientMessages;
                o.ServerMessages = this.serverMessages;
                Log.Add(o);
            }
        }

        public void SendAck(OTAck ack)
        {
            if (outgoing.Count != 0)
            {
                var msg = new OTMessage(outgoing);
                
                Task.Factory.StartNew(() => Service.SendAsync(msg));
                outgoing.Clear();
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

            var others = proxy.Register(new ClientMetaData() { Id = this.Id, Name = name});
            Array.ForEach(others, c => RegisterClient(c));

            proxy.Hello("Hello Server!");
        }


        public void ForwardChatMessage(ChatMessage msg)
        {
            if (MessageArrived != null) MessageArrived(this, msg);
        }


        public void RegisterClient(ClientMetaData meta)
        {
            if (ClientRegistered != null) ClientRegistered(this, meta);
        }
    }
}
