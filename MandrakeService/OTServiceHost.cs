using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceModelEx;
using System.ServiceModel.Activation;
using System.Reflection;


namespace Mandrake.Service
{
    public class OTServiceHost: ServiceHost
    {
        public OTServiceHost(Type t, params Uri[] baseAddresses): base(t, baseAddresses)
        {
            try
            {
                this.AddGenericResolver();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public OTServiceHost(object singletonInstance, params Uri[] baseAddresses): base(singletonInstance, baseAddresses)
        {
            this.AddGenericResolver();
        }

    }

    public class OTServiceHostFactory: ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new OTServiceHost(serviceType, baseAddresses);
        }
    }
}
