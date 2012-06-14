using umbraco.interfaces;
using UmbraCodeFirst.Attributes;
using UmbraCodeFirst.Extensions;
using UmbraCodeFirst.UI;

namespace UmbraCodeFirst.Sample.Domain.DocumentTypes
{
    [DocumentType(Name = "Base Page", Description = "The root document type, that we re-use.")]
    public abstract class BasePage : PreviewPageBase
    {
        protected BasePage(INode node) : base(node)
        {
            
        }

        [DocumentTypeProperty(DataTypeId = DefaultDataTypeDefinitions.TrueFalse, Description = "Hide page in navigations and listings", Name="Hide in navigation", Tab = typeof(DefaultTab))]
        public virtual bool UmbracoNaviHide
        {
            get { return this.GetPropertyValue(page => page.UmbracoNaviHide); }
            set { this.SetPropertyValue(page => page.UmbracoNaviHide, value); }
        }

        [DocumentTypeProperty(DataTypeId = DefaultDataTypeDefinitions.Textstring, Description = "A custom name used to create the URL of the page", Name = "URL Name", Tab = typeof(DefaultTab))]
        public virtual string UmbracoUrlName
        {
            get { return this.GetPropertyValue(page => page.UmbracoUrlName); }
            set { this.SetPropertyValue(page => page.UmbracoUrlName, value); }
        }

        [DocumentTypeProperty(DataTypeId = DefaultDataTypeDefinitions.TextboxMultiple, Description = "A comma separated list of alternative URLs for the page", Name = "URL Aliases", Tab = typeof(DefaultTab))]
        public virtual string UmbracoUrlAlias
        {
            get { return this.GetPropertyValue(page => page.UmbracoUrlAlias); }
            set { this.SetPropertyValue(page => page.UmbracoUrlAlias, value); }
        }
    }
}