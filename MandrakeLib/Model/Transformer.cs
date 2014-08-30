using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake.Model
{
    public interface ITransform
    {
        void Transform(Operation o);
        Operation Transform(Operation o1, Operation o2);
        void Clear();
    }
    public class LogTransformer : ITransform
    {
        private List<Operation> log = new List<Operation>();

        public void Transform(Operation o)
        {
            foreach (var logOp in log)
            {
                if (o.IsIndependentFrom(logOp)) o.TransformAgainst(logOp);
            }

            log.Add(o);
        }


        public void Clear()
        {
            throw new NotImplementedException();
        }


        public Operation Transform(Operation o1, Operation o2)
        {
            throw new NotImplementedException();
        }
    }

    public class StateSpaceTransformer : ITransform
    {

        public void Transform(Operation o)
        {
            throw new NotImplementedException();
        }


        public void Clear()
        {
            throw new NotImplementedException();
        }


        public Operation Transform(Operation o1, Operation o2)
        {
            throw new NotImplementedException();
        }
    }

    public class TextTransformer : ITransform
    {
        public void Transform(Operation o)
        {
            return;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }


        public Operation Transform(Operation o1, Operation o2)
        {
            Operation transformed = o2.Clone() as Operation;

            if (o1 is InsertOperation && o2 is InsertOperation)
            {
                InsertOperation oa = o1 as InsertOperation;
                InsertOperation ob = o2 as InsertOperation;

                if (oa.Position < ob.Position || (oa.Position == ob.Position && oa.OwnerId.CompareTo(ob.OwnerId) < 0))
                {
                    ((InsertOperation)transformed).Position += oa.Length;
                }
            }
            else if (o1 is InsertOperation && o2 is DeleteOperation)
            {
                InsertOperation oa = o1 as InsertOperation;
                DeleteOperation ob = o2 as DeleteOperation;

                if (oa.Position < ob.StartPosition) ((DeleteOperation)transformed).StartPosition += oa.Literal.Length;

                else if (oa.Position >= ob.StartPosition) ((DeleteOperation)transformed).EndPosition += oa.Length;                
            }
            else if (o1 is DeleteOperation && o2 is InsertOperation)
            {
                DeleteOperation oa = o1 as DeleteOperation;
                InsertOperation ob = o2 as InsertOperation;

                if (ob.Position > oa.EndPosition) ((InsertOperation)transformed).Position -= oa.Length;

                else if (ob.Position >= oa.StartPosition) ob.Literal = "";
            }
            else 
            {
                DeleteOperation oa = o1 as DeleteOperation;
                DeleteOperation ob = o2 as DeleteOperation;
                DeleteOperation copy = transformed as DeleteOperation;

                if (oa.EndPosition < ob.StartPosition)
                {
                    copy.StartPosition += oa.Length;
                    copy.EndPosition += oa.Length;
                }
                else if (ob.StartPosition < oa.EndPosition) copy.StartPosition += oa.EndPosition - ob.StartPosition;

                else if (oa.StartPosition < ob.EndPosition) copy.EndPosition -= ob.EndPosition - oa.StartPosition;
            }

            return transformed;
        }
    }
}
