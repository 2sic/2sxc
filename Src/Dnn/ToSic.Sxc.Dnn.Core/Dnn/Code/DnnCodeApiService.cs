using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Code;

/// <summary>
/// The basic DnnDynamicCode without explicitly typed model / kit
/// </summary>
[PrivateApi]
internal class DnnCodeApiService(CodeApiService.MyServices services) : DnnCodeApiService<object, ServiceKit>(services);