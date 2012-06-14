
using System.Configuration;

namespace UmbraCodeFirst.Configuration.PageFactory
{
    public class PageFactoryElement : ConfigurationElement
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

        [ConfigurationProperty("mappings", IsDefaultCollection = true)]
        public PageTypeMapElementCollection Mappings
        {
            get { return (PageTypeMapElementCollection) this["mappings"]; }
            set { this["mappings"] = value; }
        }
    }
}