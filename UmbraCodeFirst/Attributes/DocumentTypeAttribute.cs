using System;

namespace UmbraCodeFirst.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class DocumentTypeAttribute : Attribute
    {
        public DocumentTypeAttribute()
        {
            ThumbnailUrl = "folder.png";
            IconUrl = "folder.gif";
            AllowedTemplates = new Type[0];
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }
        public string IconUrl { get; set; }
        
        public Type[] AllowedChildDocumentTypes { get; set; }
        public Type[] AllowedTemplates { get; set; }

        public Type DefaultTemplate { get; set; }
    }
}