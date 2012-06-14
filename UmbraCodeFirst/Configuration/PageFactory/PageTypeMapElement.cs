using System.Configuration;

namespace UmbraCodeFirst.Configuration.PageFactory
{
    public class PageTypeMapElement : ConfigurationElement
    {
        [ConfigurationProperty("nodeTypeAlias", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string NodeTypeAlias
        {
            get
            {
                return (string)this["nodeTypeAlias"];
            }
            set
            {
                this["nodeTypeAlias"] = value;
            }
        }

        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

    }
}