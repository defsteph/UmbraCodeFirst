
using System;

namespace UmbraCodeFirst.UI.Tabs
{
    public class PreviewTab  : Tab
    {
        public override int SortOrder
        {
            get { return Int32.MinValue; }
        }

        public override string Name
        {
            get { return "Preview"; }
        }
    }
}