using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Apps;

public partial class AppDynamic
{
    #region Dynamic Properties: Configuration, Settings, Resources

    // Create config object. Note that AppConfiguration could be null, then it would use default values
    /// <inheritdoc />
    public IAppConfiguration Configuration => MyOptions.AppSpecs.Configuration;

    private ICodeDataFactory Cdf
    {
        get => field ??= cdfLazy.SetInit(obj => obj.SetFallbacks((MyOptions as AppForCodeShared)!.AppSite)).Value;
        set;
    }

    /// <summary>
    /// Replace the Cdf with one provided from outside.
    /// Not quite sure why we're doing this, was rediscovered during a refactoring 2026-10-18 2dm.
    /// My guess is that it's needed (or optimized) for the <see cref="MakeDynProperty"/>.
    /// </summary>
    /// <param name="cdf"></param>
    internal void SetupCodeDataFactory(ICodeDataFactory cdf)
        => Cdf = cdf;

    private IDynamicEntity MakeDynProperty(IEntity contents, bool propsRequired)
    {
        var wrapped = CmsEditDecorator.Wrap(contents, false);
        return Cdf.AsDynamic(wrapped, new() { ItemIsStrict = propsRequired });
    }


    public dynamic? Settings => MyOptions.SettingsEntity == null ? null : _settings.Get(() => MakeDynProperty(MyOptions.SettingsEntity, propsRequired: false));
    private readonly LazyGet<dynamic> _settings = new();

    public dynamic? Resources => MyOptions.ResourcesEntity == null ? null : _res.Get(() => MakeDynProperty(MyOptions.ResourcesEntity, propsRequired: false));
    private readonly LazyGet<dynamic> _res = new();

    #endregion


}