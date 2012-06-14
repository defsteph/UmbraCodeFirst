using System.Configuration;
using UmbraCodeFirst.Configuration.PageFactory;
using UmbraCodeFirst.Configuration.Synchronization;

/*
<!-- SECTION DECLARATION -->
<section name="umbraCodeFirst" type="UmbraCodeFirst.Configuration.UmbraCodeFirstSection" />
<! -- CONFIGURATION -->
<umbraCodeFirst>
    <synchronization enabled="true|false" />
    <pageFactory enabled="true|false">
        <mappings>
            <add nodeTypeAlias="<NodeTypeAlias>" type="<Fully Qualified Name of Type>" />
        </mappings>
    </pageFactory>
</umbraCodeFirst>
*/

namespace UmbraCodeFirst.Configuration
{
    public class UmbraCodeFirstSection : ConfigurationSection
    {
        [ConfigurationProperty("pageFactory", IsRequired = false)]
        public PageFactoryElement PageFactory
        {
            get { return (PageFactoryElement)this["pageFactory"]; }
            set { this["pageFactory"] = value; }
        }

        [ConfigurationProperty("synchronization", IsRequired = false)]
        public SynchronizationElement Synchronization
        {
            get { return (SynchronizationElement)this["synchronization"]; }
            set { this["synchronization"] = value; }
        }
    }
}
