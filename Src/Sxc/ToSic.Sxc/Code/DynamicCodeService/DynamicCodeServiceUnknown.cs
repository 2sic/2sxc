using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Code
{
    public class DynamicCodeServiceUnknown: DynamicCodeService
    {
        public DynamicCodeServiceUnknown(MyServices services, WarnUseOfUnknown<DynamicCodeServiceUnknown> _) 
            : base(services)
        {
        }
    }
}
