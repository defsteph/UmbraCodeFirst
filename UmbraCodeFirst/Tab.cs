using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UmbraCodeFirst
{
    public abstract class Tab
    {
        public abstract int SortOrder { get; }
        public abstract string Name { get; }
    }
}
