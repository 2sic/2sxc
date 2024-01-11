using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.Services.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class DynamicCodeServiceUnknown(DynamicCodeService.MyServices services, WarnUseOfUnknown<DynamicCodeServiceUnknown> _) : DynamicCodeService(services);