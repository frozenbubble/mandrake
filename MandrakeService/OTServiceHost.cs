using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceModelEx;
using System.ServiceModel.Activation;


namespace Mandrake.Service
{
    public class OTServiceHost: ServiceHost
    {
        public OTServiceHost(Type t, params Uri[] baseAddresses): base(t, baseAddresses)
        {
            this.AddGenericResolver();
            Console.WriteLine("OTServiceHost CREATED");
        }
    }

    public class OTServiceHostFactory: ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            Console.WriteLine("OTServiceHostFactory CALLED");
            return new OTServiceHost(serviceType, baseAddresses);
        }
    }
}
