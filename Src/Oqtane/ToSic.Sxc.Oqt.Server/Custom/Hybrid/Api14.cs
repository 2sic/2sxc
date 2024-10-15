using ToSic.Lib.Coding;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Oqtane specific Api base class.
///
/// It's identical to [](xref:Custom.Hybrid.Api14) but this may be enhanced in future. 
/// </summary>
[PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
public abstract class Api14 : Api12, IDynamicCode14<object, ServiceKit14>
{
    public ServiceKit14 Kit => _kit.Get(_CodeApiSvc.GetKit<ServiceKit14>);
    private readonly GetOnce<ServiceKit14> _kit = new();

    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    [PrivateApi("added in 16.05, but not sure if it should be public")]
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default)
        => CodeHlp.GetCode(path: path, className: className);

}