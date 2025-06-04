using ToSic.Eav.Apps;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Internal.Decorators;

namespace ToSic.Sxc.Apps;

public partial class App
{
    #region Dynamic Properties: Configuration, Settings, Resources

    // Create config object. Note that AppConfiguration could be null, then it would use default values
    /// <inheritdoc />
    public IAppConfiguration Configuration => AppReaderInt.Specs.Configuration;

    private dynamic MakeDynProperty(IEntity contents, bool propsRequired)
    {
        var wrapped = CmsEditDecorator.Wrap(contents, false);
        return Cdf.AsDynamic(wrapped, propsRequired: propsRequired);
    }

    internal void SetupAsConverter(ICodeDataFactory cdf) => cdfLazy.Inject(cdf);

    /// <inheritdoc cref="Eav.ImportExport.Sys.Settings" />
    public dynamic Settings => AppSettings == null ? null : _settings.Get(() => MakeDynProperty(AppSettings, propsRequired: false));
    private readonly GetOnce<dynamic> _settings = new();

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Resources => AppResources == null ? null : _res.Get(() => MakeDynProperty(AppResources, propsRequired: false));
    private readonly GetOnce<dynamic> _res = new();

    #endregion


}