using System;

namespace UmbraCodeFirst.Exceptions
{
    public class DefaultTemplateException : Exception
    {
        public DefaultTemplateException(Type defaultTemplate, Type documentType)
            : base(String.Format("The Default Template ({0}) is not one of the Allowed Templates for this Document Type ({1}).", defaultTemplate.Name, documentType.Name))
        {
            
        }
    }
}