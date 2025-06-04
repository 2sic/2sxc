using ToSic.Lib.Services;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Services.Internal;

internal class DynamicCodeServiceUnknown(DynamicCodeService.MyServices services, WarnUseOfUnknown<DynamicCodeServiceUnknown> _) : DynamicCodeService(services);