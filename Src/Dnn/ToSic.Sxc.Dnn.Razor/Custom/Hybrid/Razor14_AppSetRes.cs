using ToSic.Sxc.Apps;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

abstract partial class Razor14
{
    /// <inheritdoc cref="IDynamicCode.App" />
    public new IApp App => CodeApi.App;

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Resources => CodeApi.Resources;

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Settings => CodeApi.Settings;

}