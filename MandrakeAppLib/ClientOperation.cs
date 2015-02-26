using Mandrake.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake.Sample.Client.Operations
{
    /// <summary>
    /// Base class for operations that refer to a single position in the document
    /// </summary>
    [DataContract]
    public abstract class SinglePositionOperation : Operation
    {
        public SinglePositionOperation() { }

        /// <summary>
        /// Gets the position of the operation
        /// </summary>
        [DataMember]
        public int Position { get; set; }
    }

    [DataContract]
    public abstract class IntervalOperation : Operation
    {
        public IntervalOperation(int startPosition, int endPosition) 
        {
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
        }

        [DataMember]
        public int StartPosition { get; set; }
        [DataMember]
        public int EndPosition { get; set; }
    }

    [DataContract]
    public class InsertOperation : SinglePositionOperation
    {
        [DataMember]
        public string Literal { get; set; }

        //[DataMember]
        public int Length
        {
            get { return Literal.Length; }
        }

        public InsertOperation(int position, string literal): base()
        {
            this.Position = position;
            this.Literal = literal;
        }

        public override string ToString()
        {
            return String.Format("Insert[{0}, {1}]", this.Literal, this.Position);
        }

        public override object Clone()
        {
            var o = new InsertOperation(this.Position, this.Literal);
            Initialize(o);

            return o;
        }
    }

    [DataContract]
    public class DeleteOperation : IntervalOperation
    {
        public int Length
        {
            get { return EndPosition - StartPosition; }
        }

        public DeleteOperation(int startPosition, int endPosition): base(startPosition, endPosition) { }

        public override string ToString()
        {
            return String.Format("Delete[{0}, {1}]", this.StartPosition, this.EndPosition);
        }

        public override object Clone()
        {
            var o = new DeleteOperation(this.StartPosition, this.EndPosition);
            Initialize(o);

            return o;
        }
    }

    [DataContract]
    public class CaretPositionOperation: SinglePositionOperation
    {
        public CaretPositionOperation(int offset)
        {
            this.Position = offset;
        }

        public override string ToString()
        {
            return String.Format("Move[{0}]", this.Position);
        }

        public override object Clone()
        {
            var o = new CaretPositionOperation(this.Position);
            Initialize(o);

            return o;
        }
    }

    [DataContract]
    public class SelectionOperation: IntervalOperation
    {
        public SelectionOperation(int startPosition, int endPosition): base(startPosition, endPosition) { }

        public override object Clone()
        {
            var o = new SelectionOperation(this.StartPosition, this.EndPosition);
            Initialize(o);

            return o;
        }
    }
}
