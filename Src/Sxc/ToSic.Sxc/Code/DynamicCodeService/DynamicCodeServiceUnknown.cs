using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Code
{
    public class DynamicCodeServiceUnknown: DynamicCodeService
    {
        public DynamicCodeServiceUnknown(Dependencies dependencies, WarnUseOfUnknown<DynamicCodeServiceUnknown> warn) 
            : base(dependencies)
        {
        }
    }
}
