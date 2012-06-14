using System;
using System.Web;

namespace UmbraCodeFirst.Factories
{
    public class HttpContextFactory
    {
        private static HttpContextBase _context;
        public static HttpContextBase Current
        {
            get
            {
                if (_context != null)
                    return _context;

                if (HttpContext.Current == null)
                    throw new InvalidOperationException("HttpContext not available");

                SetCurrentContext(new HttpContextWrapper(HttpContext.Current));

                return _context;
            }
        }

        public static void SetCurrentContext(HttpContextBase context)
        {
            _context = context;
        }
    }
}
