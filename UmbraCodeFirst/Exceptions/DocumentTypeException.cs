using System;

namespace UmbraCodeFirst.Exceptions
{
    public class DocumentTypeException : Exception
    {
        public DocumentTypeException() : base("The type must inherit from UmbracoModelBase.")
        {
            
        }
    }
}