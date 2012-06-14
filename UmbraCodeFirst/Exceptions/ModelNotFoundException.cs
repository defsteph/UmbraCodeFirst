using System;

namespace UmbraCodeFirst.Exceptions
{
    public class ModelNotFoundException : Exception
    {
        public ModelNotFoundException() : base("Cannot find page without an ID.") { }
    }
}