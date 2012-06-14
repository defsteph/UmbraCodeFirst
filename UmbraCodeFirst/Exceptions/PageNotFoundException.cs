using System;

namespace UmbraCodeFirst.Exceptions
{
    public class PageNotFoundException : Exception
    {
        public PageNotFoundException() : base("Cannot find page without an ID.") { }
    }
}