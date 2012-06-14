
using System.Configuration;

namespace UmbraCodeFirst.Configuration.Synchronization
{
    public class SynchronizationElement : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = "true", IsRequired = false)]
        public bool Enabled
        {
            get
            {
                return (bool)this["enabled"];
            }
            set
            {
                this["enabled"] = value;
            }
        }
    }
}