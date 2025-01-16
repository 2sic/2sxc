using ToSic.Eav.Apps;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Decorators;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.Apps;

public partial class App
{
    #region Dynamic Properties: Configuration, Settings, Resources

    // Create config object. Note that AppConfiguration could be null, then it would use default values
    /// <inheritdoc />
    public IAppConfiguration Configuration => AppReaderInt.Specs.Configuration;

    private DynamicEntity MakeDynProperty(IEntity contents, bool propsRequired)
    {
        var wrapped = CmsEditDecorator.Wrap(contents, false);
        return Cdf.AsDynamic(wrapped, propsRequired: propsRequired);
    }

    internal void SetupAsConverter(CodeDataFactory cdf) => cdfLazy.Inject(cdf);

    /// <inheritdoc cref="IDynamicCode12.Settings" />
    public dynamic Settings => AppSettings == null ? null : _settings.Get(() => MakeDynProperty(AppSettings, propsRequired: false));
    private readonly GetOnce<dynamic> _settings = new();

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Resources => AppResources == null ? null : _res.Get(() => MakeDynProperty(AppResources, propsRequired: false));
    private readonly GetOnce<dynamic> _res = new();

    #endregion


}