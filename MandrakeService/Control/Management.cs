using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using Mandrake.Model;
using Mandrake.Service;
//using Mandrake.Controls;
using System;
using System.Collections.Generic;

namespace Mandrake.Management
{
    //public delegate void OperationActionEventHandler(object sender, Operation o);

    //public class ClientManager : OTManager, IOTCallback
    //{
    //    private List<Operation> outgoing = new List<Operation>();
    //    private bool acknowledged = true;

    //    public event OperationActionEventHandler OperationPerformed;

    //    public Guid Id { get; set; }
    //    public IOTAwareService Service { get; set; }

    //    public ClientManager(IOTAwareContext context)
    //    {
    //        ManagerChain = new List<IOperationManager>();
    //        Log = new List<Operation>();
    //        this.context = context;
    //    }

    //    public void OnChange(object sender, EventArgs e)
    //    {
    //        if (!context.IsUpdatedByUser) return;

    //        Operation o = null;

    //        foreach (var manager in ManagerChain)
    //        {
    //            if ((o = manager.TryRecognize(sender, e)) != null)
    //            {
    //                o.OwnerId = Id;

    //                if (OperationPerformed != null)
    //                {
    //                    TrySend(o);
    //                    myMessages++;
    //                }

    //                else return;
    //            }
    //        }
    //    }

    //    private void TrySend(Operation o)
    //    {
    //        if (acknowledged)
    //        {
    //            //Service.Send(new OTMessage(myMessages, otherMessages, o));
    //            acknowledged = false;
    //        }

    //        else outgoing.Add(o);
    //    }

    //    public void OnOperationPerformed(object sender, Operation o)
    //    {
    //        Execute(o);
    //    }

    //    protected override void Execute(Operation o)
    //    {
    //        Transform(o);

    //        foreach (var manager in ManagerChain)
    //        {
    //            if (manager.TryExecute(context, o))
    //            {
    //                o.ExecutedAt = DateTime.Now;
    //                Log.Add(o);
    //                return;
    //            }
    //        }
    //    }

    //    protected override void Transform(Operation o)
    //    {
    //        foreach (var logOp in Log)
    //        {
    //            if (o.IsIndependentFrom(logOp)) o.TransformAgainst(logOp);
    //        }
    //    }

    //    public void Forward(OTMessage message)
    //    {
    //        //for now
    //        //Execute(message.Content);
    //    }

    //    public void SendAck(OTMessage message)
    //    {
    //        if (outgoing.Count != 0)
    //        {
    //            //Service.Send(new OTMessage(myMessages, otherMessages, o));
    //            outgoing.Clear();
    //        }

    //        else acknowledged = true;
    //    }


    //    public void Echo(string msg)
    //    {
    //        Console.WriteLine(this.Id + " got: " + msg);
    //    }
    //}    

}