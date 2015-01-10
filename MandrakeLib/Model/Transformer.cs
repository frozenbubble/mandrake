using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake.Model
{
    public interface ITransform
    {
        //void Transform(Operation o);
        Operation Transform(Operation o1, Operation o2);
        void Clear();
    }

    public class TransformationException : System.Exception
    {
        public TransformationException(String message) : base(message) { }
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

            if (o1 is InsertOperation && o2 is InsertOperation) Transform((InsertOperation)o1, (InsertOperation)o2);
            
            else if (o1 is InsertOperation && o2 is DeleteOperation) Transform((InsertOperation)o1, (DeleteOperation)o2);             
            
            else if (o1 is DeleteOperation && o2 is InsertOperation) Transform((DeleteOperation)o1, (InsertOperation)o2);

            else if (o1 is DeleteOperation && o2 is DeleteOperation) Transform((DeleteOperation)o1, (DeleteOperation)o2);

            return transformed;
        }
        private static void Transform(InsertOperation o1, InsertOperation o2)
        {
            if (o1.Position < o2.Position || (o1.Position == o2.Position && o1.CreatedAt < o2.CreatedAt))
            {
                o2.Position += o1.Length;
            }
        }
        private static void Transform(InsertOperation o1, DeleteOperation o2)
        {
            if (o1.Position < o2.StartPosition) o2.StartPosition += o1.Literal.Length;

            else if (o1.Position >= o2.StartPosition) o2.EndPosition += o1.Length;
        }

        private static void Transform(DeleteOperation o1, InsertOperation o2)
        {
            if (o2.Position > o1.EndPosition) o2.Position -= o1.Length;

            else if (o2.Position >= o1.StartPosition) o2.Literal = "";
        }

        private static void Transform(DeleteOperation oa, DeleteOperation ob)
        {
            if (oa.EndPosition < ob.StartPosition)
            {
                ob.StartPosition += oa.Length;
                ob.EndPosition += oa.Length;
            }
            else if (ob.StartPosition >= oa.StartPosition && oa.EndPosition >= ob.StartPosition) ob.EndPosition = ob.StartPosition;

            else if (oa.StartPosition >= ob.StartPosition && ob.EndPosition >= oa.StartPosition) ob.EndPosition -= oa.Length;

            else if (ob.StartPosition < oa.EndPosition) ob.StartPosition += oa.EndPosition - ob.StartPosition;

            else if (oa.StartPosition < ob.EndPosition) ob.EndPosition -= ob.EndPosition - oa.StartPosition;
        }

    }

    class ShapeTransformer : ITransform
    {
        public void Transform(Operation o)
        {
            throw new NotImplementedException();
        }

        public Operation Transform(Operation o1, Operation o2)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }

}
