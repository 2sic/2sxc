using ToSic.Sxc.Apps;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

abstract partial class Razor14
{
    /// <inheritdoc cref="IDynamicCodeDocs.App" />
    public new IApp App => CodeApi.App;

    /// <inheritdoc cref="IDynamicCode12Docs.Resources" />
    public dynamic Resources => CodeApi.Resources;

    /// <inheritdoc cref="IDynamicCode12Docs.Resources" />
    public dynamic Settings => CodeApi.Settings;

}