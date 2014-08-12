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
    }
}
