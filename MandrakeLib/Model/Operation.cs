using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake.Model
{
    [DataContract]
    [KnownType(typeof(IntervalOperation))]
    [KnownType(typeof(SinglePositionOperation))]
    public abstract class Operation
    {
        [DataMember]
        public Guid OwnerId { get; set; }
        [DataMember]
        public DateTime CreatedAt { get; set; }
        [DataMember]
        public DateTime ExecutedAt { get; set; }

        public Operation()
        {
            this.CreatedAt = DateTime.Now;
            this.ExecutedAt = DateTime.MaxValue;
        }

        /// <summary>
        /// Transforms the operation against another operation updating its referenced positions.
        /// </summary>
        /// <param name="o">the operation to transform against</param>
        public abstract void TransformAgainst(Operation o);
        public abstract int GetNewPosition(int position);

        public bool IsPreceedingTo(Operation o)
        {
            if (this.OwnerId == o.OwnerId) return this.CreatedAt < o.CreatedAt;

            else return this.ExecutedAt < o.CreatedAt;
        }

        public bool IsIndependentFrom(Operation o)
        {
            return !(this.IsPreceedingTo(o) || o.IsPreceedingTo(this));
        }
    }

    /// <summary>
    /// Base class for operations that refer to a single position in the document
    /// </summary>
    [DataContract]
    [KnownType(typeof(InsertOperation))]
    public abstract class SinglePositionOperation : Operation
    {
        public SinglePositionOperation() : base() { }

        /// <summary>
        /// Gets the position of the operation
        /// </summary>
        [DataMember]
        public int Position { get; protected set; }
    }

    [DataContract]
    [KnownType(typeof(DeleteOperation))]
    public abstract class IntervalOperation : Operation
    {
        public IntervalOperation() : base() { }

        [DataMember]
        public int StartPosition { get; protected set; }
        [DataMember]
        public int EndPosition { get; protected set; }
    }

    [DataContract]
    public class InsertOperation : SinglePositionOperation
    {
        [DataMember]
        public string Literal { get; private set; }

        public InsertOperation(int position, string literal)
            : base()
        {
            this.Position = position;
            this.Literal = literal;
        }

        public override void TransformAgainst(Operation o)
        {
            this.Position = o.GetNewPosition(this.Position);
        }

        public override int GetNewPosition(int position)
        {
            if (position >= this.Position) return position + Literal.Length;

            else return position;
        }

        public override string ToString()
        {
            return String.Format("Insert[{0}, {1}]", this.Literal, this.Position);
        }
    }

    [DataContract]
    public class DeleteOperation : IntervalOperation
    {
        public DeleteOperation(int startPosition, int endPosition)
            : base()
        {
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
        }

        public override void TransformAgainst(Operation o)
        {
            this.StartPosition = o.GetNewPosition(this.StartPosition);
            this.EndPosition = o.GetNewPosition(this.EndPosition);
        }

        public override int GetNewPosition(int position)
        {
            if (position < this.StartPosition) return position;

            else if (position > this.EndPosition) return position - (this.EndPosition - this.StartPosition);

            else return this.StartPosition;
        }

        public override string ToString()
        {
            return String.Format("Delete[{0}, {1}]", this.StartPosition, this.EndPosition);
        }
    }

}
