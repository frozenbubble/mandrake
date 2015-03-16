using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake.Model
{
    [DataContract]
    //[KnownType(typeof(IntervalOperation))]
    //[KnownType(typeof(SinglePositionOperation))]
    public abstract class Operation: ICloneable
    {
        [DataMember]
        public Guid OwnerId { get; set; }
        [DataMember]
        public string DocumentName { get; set; }
        [DataMember]
        public DateTime CreatedAt { get; set; }
        [DataMember]
        public DateTime ExecutedAt { get; set; }
        [DataMember]
        public long ClientMessages { get; set; }
        [DataMember]
        public long ServerMessages { get; set; }

        public Operation()
        {
            this.CreatedAt = DateTime.Now;
            this.ExecutedAt = DateTime.MaxValue;
        }

        public Operation(Operation o)
        {
            this.CreatedAt = o.CreatedAt;
            this.ExecutedAt = o.ExecutedAt;
        }

        public abstract object Clone();

        protected void Initialize(Operation o)
        {
            o.ExecutedAt = this.ExecutedAt;
            o.CreatedAt = this.CreatedAt;
            o.OwnerId = this.OwnerId;
            o.ClientMessages = this.ClientMessages;
            o.ServerMessages = this.ServerMessages;
        }

        protected static Operation Prototype<T>(T op) where T: Operation, new()
        {
            Operation o = new T();
            o.ExecutedAt = op.ExecutedAt;
            o.CreatedAt = op.CreatedAt;
            o.OwnerId = op.OwnerId;
            o.ClientMessages = op.ClientMessages;
            o.ServerMessages = op.ServerMessages;
            
            return o;
        }

    }

    [DataContract]
    public class AggregateOperation : Operation
    {
        public List<Operation> Operations { get; set; }

        public AggregateOperation()
        {
            Operations = new List<Operation>();
        }

        public AggregateOperation(IEnumerable<Operation> operations)
        {
            Operations = new List<Operation>(operations);
        }

        public override object Clone()
        {
            var o = new AggregateOperation(this.Operations);
            Initialize(o);

            return o;
        }

        public override string ToString()
        {
            if (Operations.Count != 0) return String.Format("{0} : {1} : {2}",
                Operations.Last().ExecutedAt.Hour, Operations.Last().ExecutedAt.Minute, Operations.Last().ExecutedAt.Second);

            else return "No operation";
        }
    }
}
