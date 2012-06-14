using System;
using umbraco.interfaces;
using UmbraCodeFirst.Attributes;
using UmbraCodeFirst.Extensions;
using UmbraCodeFirst.Sample.Domain.Tabs;
using UmbraCodeFirst.Sample.masterpages;

namespace UmbraCodeFirst.Sample.Domain.DocumentTypes
{
    [DocumentType(Name = "Standard Page", Description = "A standard page.", IconUrl = "doc.gif", ThumbnailUrl = "docWithImage.png", AllowedChildDocumentTypes = new[]{typeof(StandardPage)}, DefaultTemplate = typeof(Inherited), AllowedTemplates = new[]{typeof(Inherited)})]
    public class StandardPage : BasePage
    {
        public StandardPage(INode node) : base(node)
        {

        }

        [DocumentTypeProperty(DataTypeId = DefaultDataTypeDefinitions.Textstring, Description = "The page heading", Name = "Heading", Tab = typeof(ContentTab), SortOrder = 10)]
        public virtual string Heading
        {
            get
            {
                var heading = this.GetPropertyValue(page => page.Heading);
                return String.IsNullOrWhiteSpace(heading) ? NodeName : heading;
            }
            set { this.SetPropertyValue(page => page.Heading, value); }
        }

        [DocumentTypeProperty(DataTypeId = DefaultDataTypeDefinitions.RichtextEditor, Description = "The page main body", Name = "Text", Tab = typeof(ContentTab), SortOrder = 100)]
        public virtual string BodyText
        {
            get { return this.GetPropertyValue(page => page.BodyText); }
            set { this.SetPropertyValue(page => page.BodyText, value); }
        }

        [DocumentTypeProperty(DataTypeId = DefaultDataTypeDefinitions.Textstring, Description = "An email address", Name = "Email", Tab = typeof(ContentTab), SortOrder = 1, ValidationExpression = @"^\b[\w\.-]+@[\w\.-]+\.\w{2,4}\b$")]
        public virtual string EmailAddress
        {
            get { return this.GetPropertyValue(page => page.EmailAddress); }
            set { this.SetPropertyValue(page => page.EmailAddress, value); }
        }

    }
}