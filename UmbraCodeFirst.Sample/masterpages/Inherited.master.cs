using System;
using UmbraCodeFirst.Attributes;
using UmbraCodeFirst.UI;

namespace UmbraCodeFirst.Sample.masterpages
{
    [DocumentTypeTemplate(Name = "Inherited Template", MasterTemplate = typeof(Master))]
    public partial class Inherited : MasterPageBase<UmbracoPageBase>
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}