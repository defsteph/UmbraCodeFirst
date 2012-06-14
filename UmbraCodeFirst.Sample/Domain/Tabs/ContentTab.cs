
namespace UmbraCodeFirst.Sample.Domain.Tabs
{
    public class ContentTab  : Tab
    {
        public override int SortOrder
        {
            get { return 10; }
        }

        public override string Name
        {
            get { return "Content"; }
        }
    }
}