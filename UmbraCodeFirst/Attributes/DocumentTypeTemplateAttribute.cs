using System;

namespace UmbraCodeFirst.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class DocumentTypeTemplateAttribute : Attribute
    {
        public virtual string Name { get; set; }
        public virtual Type MasterTemplate { get; set; }
    }
}
