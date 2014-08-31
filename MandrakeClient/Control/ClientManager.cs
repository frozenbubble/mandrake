using Mandrake.Model;
using System.Collections.Generic;
using Mandrake.Client.OTServiceReference;
using System;
using Mandrake.View.Controls;
using Mandrake.Service;
using System.Threading.Tasks;

namespace Mandrake.Management.Client
{
    public delegate void OperationActionEventHandler(object sender, Operation o);

    public class ClientManager : OTManager, IOTAwareServiceCallback
    {
        private List<Operation> outgoing;
        private bool acknowledged = true;
        private ISynchronize syncManager;
        private ITransform transformer;

        public event OperationActionEventHandler OperationPerformed;

        private bool Acknowledged
        {
            get { return acknowledged; }
            set { acknowledged = value; }
        }
        public Guid Id { get; set; }
        public Mandrake.Client.OTServiceReference.IOTAwareService Service { get; set; }

        public ClientManager(IOTAwareContext context)
        {
            outgoing = new List<Operation>();
            ManagerChain = new List<IOperationManager>(); // Autofac
            //syncManager = new TextEditorSynchronizer((OTAwareDocument) context); // Autofac
            Log = new List<Operation>();
            this.Context = context; // Autofac
            transformer = new LogTransformer();     //autofac
            //transformer = new TextTransformer();
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
                    o.OwnerId = Id;
                    TrySend(o);
                    myMessages++;
                }
            }
        }

        private void TrySend(Operation o)
        {
            if (Acknowledged)
            {
                Acknowledged = false;
                
                Parallel.Invoke(() => Service.Send(new OTMessage(myMessages, otherMessages, o)));
            }

            else outgoing.Add(o);
        }

        public void OnOperationPerformed(object sender, Operation o)
        {
            Execute(o);
        }

        protected override void Execute(Operation o)
        {
            Transform(o);

            foreach (var manager in ManagerChain)
            {
                if (manager.TryExecute(Context, o))
                {
                    o.ExecutedAt = DateTime.Now;
                    Log.Add(o);
                    return;
                }
            }
        }

        protected override void Transform(Operation o)
        {
            foreach (var logOp in Log)
            {
                if (o.IsIndependentFrom(logOp)) o.TransformAgainst(logOp);
            }
        }

        public void Forward(OTMessage message)
        {
            //old
            foreach (var o in message.Content) Execute(o);

            //new
            foreach (var o in message.Content)
            {

            }
        }

        public void SendAck(OTMessage message)
        {
            if (outgoing.Count != 0)
            {
                Service.Send(new OTMessage(myMessages, otherMessages, outgoing));
                outgoing.Clear();
            }

            else Acknowledged = true;
        }

        public void Echo(string msg)
        {
            Console.WriteLine(this.Id + " got: " + msg);
        }
    }
}
