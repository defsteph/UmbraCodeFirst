using System;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.propertytype;
using umbraco.interfaces;

namespace UmbraCodeFirst
{
    internal class DocumentProperty : Property, IProperty
    {
        public DocumentProperty(int id, PropertyType pt) : base(id, pt) { }
        public DocumentProperty(int id) : base(id) { }

        #region Implementation of IProperty

        public string Alias
        {
            get { return PropertyType.Alias; }
        }

        public new string Value
        {
            get { return base.Value.ToString(); }
        }

        public Guid Version
        {
            get { return VersionId; }
        }

        #endregion
    }
}