using System.Diagnostics.CodeAnalysis;
using Custom.Razor.Sys;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

// ReSharper disable once UnusedMember.Global
[PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
public abstract class Razor14: Razor12<dynamic>, IRazor14<object, ServiceKit14>
{
    [field: AllowNull, MaybeNull]
    public ServiceKit14 Kit => field ??= CodeApi.ServiceKit14;

    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    [PrivateApi("added in 16.05, but not sure if it should be public")]
    public dynamic? GetCode(string path, NoParamOrder noParamOrder = default, string? className = default)
        => RzrHlp.GetCode(path, noParamOrder, className);

}