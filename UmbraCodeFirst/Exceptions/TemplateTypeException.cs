using System;

namespace UmbraCodeFirst.Exceptions
{
    public class TemplateTypeException : Exception
    {
        public TemplateTypeException() : base("A template type must be a MasterPage")
        {
            
        }
    }
}