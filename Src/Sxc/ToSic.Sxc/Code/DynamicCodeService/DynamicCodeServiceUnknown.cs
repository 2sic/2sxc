using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Code
{
    public class DynamicCodeServiceUnknown: DynamicCodeService
    {
        public DynamicCodeServiceUnknown(Dependencies services, WarnUseOfUnknown<DynamicCodeServiceUnknown> _) 
            : base(services)
        {
        }
    }
}
