using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Code
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class DynamicCodeServiceUnknown: DynamicCodeService
    {
        public DynamicCodeServiceUnknown(MyServices services, WarnUseOfUnknown<DynamicCodeServiceUnknown> _) 
            : base(services)
        {
        }
    }
}
