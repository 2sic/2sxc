using System.Diagnostics.CodeAnalysis;
using Custom.Razor.Sys;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

// ReSharper disable once UnusedMember.Global
public abstract class Razor14: Razor12<dynamic>, IRazor14<object, ServiceKit14>
{
    [field: AllowNull, MaybeNull]
    public ServiceKit14 Kit => field ??= CodeApi.ServiceKit14;

    /// <inheritdoc cref="ITypedCode16.GetCode"/>
    public dynamic? GetCode(string path, NoParamOrder noParamOrder = default, string? className = default)
        => RzrHlp.GetCode(path, noParamOrder, className);

}