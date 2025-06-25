using ToSic.Lib.Services;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Services.Sys.DynamicCodeService;

internal class DynamicCodeServiceUnknown(DynamicCodeService.MyServices services, WarnUseOfUnknown<DynamicCodeServiceUnknown> _) : DynamicCodeService(services);