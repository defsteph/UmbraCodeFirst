using System.Configuration;

namespace UmbraCodeFirst.Configuration.PageFactory
{
    [ConfigurationCollection(typeof(PageTypeMapElement))]  
    public class PageTypeMapElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PageTypeMapElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PageTypeMapElement) element).NodeTypeAlias;
        }
    }
}