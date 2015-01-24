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
        //void Clear();
    }

    public class TransformationException : System.Exception
    {
        public TransformationException(String message) : base(message) { }
    }
}
