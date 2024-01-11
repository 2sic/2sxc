using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Code;

/// <summary>
/// The basic DnnDynamicCode without explicitly typed model / kit
/// </summary>
[PrivateApi]
internal class DnnDynamicCodeRoot(DynamicCodeRoot.MyServices services)
    : DnnDynamicCodeRoot<object, ServiceKit>(services);