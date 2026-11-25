
#pragma warning disable IDE0130
namespace Custom.Hybrid;
#pragma warning restore IDE0130

abstract partial class Razor14
{
    /// <inheritdoc cref="IDynamicCodeDocs.App" />
    public new IAppDynamic App => CodeApi.App;

    /// <inheritdoc cref="IDynamicCode12Docs.Resources" />
    public dynamic Resources => CodeApi.Resources;

    /// <inheritdoc cref="IDynamicCode12Docs.Resources" />
    public dynamic Settings => CodeApi.Settings;

}