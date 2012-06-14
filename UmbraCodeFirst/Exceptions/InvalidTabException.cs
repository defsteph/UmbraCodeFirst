using System;

namespace UmbraCodeFirst.Exceptions
{
    public class InvalidTabTypeException : Exception
    {
        public InvalidTabTypeException(Type type)
            : base(String.Format("The specified type ({0}) must inherit from UmbraCodeFirst.Tab", type.FullName))
        {
            
        }
    }
}