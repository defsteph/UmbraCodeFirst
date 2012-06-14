
namespace UmbraCodeFirst
{
    public sealed class DefaultTab : Tab
    {
        public override int SortOrder
        {
            get { return 0; }
        }

        public override string Name
        {
            get { return "Generic Properties"; }
        }
    }
}