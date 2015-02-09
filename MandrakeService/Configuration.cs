using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake.Service.Configuration
{
    public class ServiceContextConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("Context", IsRequired = false, IsDefaultCollection = true)]
        public ContextElement Context
        {
            get { return (ContextElement)this["Context"]; }
            set { this["Context"] = value; }
        }
    }

    public class ContextElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsKey = true, IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }

        [ConfigurationProperty("assembly", IsKey = true, IsRequired = true)]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }
    }
}
