using ToSic.Sxc.Apps;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

abstract partial class Razor14
{
    /// <inheritdoc cref="IDynamicCode.App" />
    public new IApp App => _CodeApiSvc.App;

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Resources => _CodeApiSvc.Resources;

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Settings => _CodeApiSvc.Settings;

}