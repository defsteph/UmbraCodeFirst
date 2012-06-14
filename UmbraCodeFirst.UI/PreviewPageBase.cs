using UmbraCodeFirst.Attributes;
using UmbraCodeFirst.Extensions;
using UmbraCodeFirst.UI.Tabs;
using umbraco.interfaces;

namespace UmbraCodeFirst.UI
{
    public class PreviewPageBase : UmbracoPageBase
    {
        protected PreviewPageBase(INode node)
            : base(node)
        {
            
        }

        [DocumentTypeProperty(DataTypeId = DataTypes.Preview.PreviewDataType.DataTypeNodeId, Description = "A preview of the page", Name = "Preview", Tab = typeof(PreviewTab))]
        public virtual int Preview
        {
            get { return this.GetPropertyValue(page => page.Preview); }
            set { this.SetPropertyValue(page => page.Preview, value); }
        }

    }
}
