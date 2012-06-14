using System;

namespace UmbraCodeFirst.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DocumentTypePropertyAttribute : Attribute
    {
        public DocumentTypePropertyAttribute()
        {
            Tab = typeof (DefaultTab);
        }

        public string Name { get; set; }
        public int DataTypeId { get; set; }
        public Type Tab { get; set; }
        public bool Mandatory { get; set; }
        public string ValidationExpression { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }

    }
}