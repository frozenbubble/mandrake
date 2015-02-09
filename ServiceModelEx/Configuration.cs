using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ServiceModelEx.Extensions
{
    public class AssemblyLookupConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired=false, IsDefaultCollection=true)]
        public AssemblyCollection Assemblies
        {
            get { return (AssemblyCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    public class AssemblyCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AssemblyInstanceElement)element).Name;
        }
    }

    public class AssemblyInstanceElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name 
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }
    }
}
