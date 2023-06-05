using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Data
{
    public partial class TypedItem
    {
        public class MyHelpers
        {
            public AdamManager AdamManager { get; }
            public DynamicEntity.MyServices Services { get; }

            public MyHelpers(AdamManager adamManager, DynamicEntity.MyServices services)
            {
                AdamManager = adamManager;
                Services = services;
            }
        }
    }
}
