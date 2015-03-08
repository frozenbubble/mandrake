using Mandrake.Model;
using Mandrake.Sample.Client.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandrakeAppLib
{
    public static class OperationCompressor
    {
        public static IEnumerable<Operation> Compress(IEnumerable<Operation> operations)
        {
            var buffer = new List<Operation>();
            var compressed = new List<Operation>();
            Operation prev = null;

            var relevant = operations.Where(o => o is InsertOperation || o is DeleteOperation);

            foreach (var current in relevant)
            {
                if (Neighbours(prev, current)) buffer.Add(current);

                else
                {
                    compressed.Add(buffer.Compress());
                    buffer.Clear();
                }

                prev = current;
            }

            return compressed;
        }

        private static Operation Compress(this List<Operation> ops)
        {
            Operation prev = null;

            foreach (var o in ops)
            {
                if (o is InsertOperation) prev = Compress((InsertOperation)prev, (InsertOperation)o);

                if (o is DeleteOperation) prev = Compress((DeleteOperation)prev, (DeleteOperation)o);
            }

            return prev;
        }


        private static Operation Compress(InsertOperation o1, InsertOperation o2)
        {
            if (o1 == null) return o2;

            var o = (InsertOperation)o1.Clone();
            o1.Literal = o1.Literal + o2.Literal;

            return o;
        }

        private static Operation Compress(DeleteOperation o1, DeleteOperation o2)
        {
            if (o1 == null) return o2;

            var o = (DeleteOperation)o1.Clone();
            o.EndPosition = o2.EndPosition;

            return o;
        }

        private static bool Neighbours(Operation prev, Operation current)
        {
            if (prev == null) return true;

            if (prev.GetType() != current.GetType() || !prev.OwnerId.Equals(current.OwnerId)) return false;

            if (prev is InsertOperation)
            {
                var o1 = prev as InsertOperation;
                var o2 = current as InsertOperation;

                return (o1.Position + o1.Length == o2.Position || o2.Position + o2.Length == o1.Position);
            }
            else
            {
                var o1 = prev as DeleteOperation;
                var o2 = current as DeleteOperation;

                return (o1.EndPosition == o2.EndPosition || o2.EndPosition == o1.StartPosition);
            }
        }
    }
}
