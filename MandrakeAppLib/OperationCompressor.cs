using Mandrake.Client.View;
using Mandrake.Model;
using Mandrake.Sample.Client.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake.Sample.Client.Util
{
    public static class OperationCompressor
    {
        private static MultiCaretTextEditor editor = new MultiCaretTextEditor();
        public static IEnumerable<Operation> Compress(IEnumerable<Operation> operations)
        {
            var buffer = new List<Operation>();
            var compressed = new List<Operation>();
            Operation prev = null;

            var relevant = operations.Where(o => o is InsertOperation || o is DeleteOperation);

            foreach (var current in relevant)
            {
                if (Neighbours(prev, current) || buffer.Count == 0) buffer.Add(current);

                else
                {
                    compressed.Add(new AggregateOperation(buffer));
                    buffer.Clear();
                    buffer.Add(current);
                }

                prev = current;
            }

            if (buffer.Count != 0) compressed.Add(new AggregateOperation(buffer));

            return compressed;
        }

        private static bool Neighbours(Operation prev, Operation current)
        {
            if (prev == null) return true;

            if (prev.GetType() != current.GetType() || !prev.OwnerId.Equals(current.OwnerId)) return false;

            if (prev is InsertOperation)
            {
                var o1 = prev as InsertOperation;
                var o2 = current as InsertOperation;

                return (o1.Position + o1.Length == o2.Position || o2.Position == o1.Position);
            }
            else
            {
                var o1 = prev as DeleteOperation;
                var o2 = current as DeleteOperation;

                return (o1.StartPosition == o2.EndPosition);
            }
        }
    }
}
