using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandrake.Model;
using Mandrake.View.Controls;
using Mandrake.Service;
using Mandrake.Client.OTServiceReference;

namespace Mandrake.Management.Client
{

    public delegate void OperationActionEventHandler(object sender, Operation o);

    public class ClientManager : OTManager, IOTAwareServiceCallback
    {
        protected int clientMessages;

        private List<Operation> outgoing = new List<Operation>();
        private bool acknowledged = true;

        public event OperationActionEventHandler OperationPerformed;

        public Guid Id { get; set; }
        public Mandrake.Client.OTServiceReference.IOTAwareService Service { get; set; }

        public ClientManager(IOTAwareContext context)
        {
            outgoing = new List<Operation>();
            ManagerChain = new List<IOperationManager>(); // Autofac
            //syncManager = new TextEditorSynchronizer((OTAwareDocument) context); // Autofac
            Log = new List<Operation>();
            this.Context = context; // Autofac
            //transformer = new LogTransformer();     //autofac
            transformer = new TextTransformer();
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
                }
            }
        }

        private void TrySend(Operation o)
        {
            if (acknowledged)
            {
                acknowledged = false;

                Parallel.Invoke(() => Service.Send(new OTMessage(o)));
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

            //foreach (var op in outgoing)
            //{
            //    transformer.Transform(op, o);

            //    transformer.Transform(copy, op);
            //    op.ServerMessages++;
            //}
        }

        public void Forward(OTMessage message)
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
            //TODO: throw away acknowledged messages from log ?

            if (outgoing.Count != 0)
            {
                Service.Send(new OTMessage(outgoing));
                outgoing.Clear();
            }

            else acknowledged = true;
        }

        public void Echo(string msg)
        {
            Console.WriteLine(this.Id + " got: " + msg);
        }
    }
}
