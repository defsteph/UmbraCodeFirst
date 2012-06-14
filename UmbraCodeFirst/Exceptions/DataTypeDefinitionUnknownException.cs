using System;

namespace UmbraCodeFirst.Exceptions
{
    public class DataTypeDefinitionUnknownException : Exception
    {
        public DataTypeDefinitionUnknownException(int id)
            : base(String.Format("No DataTypeDefinition with ID {0} could be found. Please make sure it is registered.", id))
        {
            
        }
    }
}