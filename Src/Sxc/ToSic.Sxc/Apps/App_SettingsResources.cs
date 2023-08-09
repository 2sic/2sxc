using ToSic.Eav.Data;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Decorators;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    public partial class App
    {
        #region Dynamic Properties: Configuration, Settings, Resources

        /// <inheritdoc />
        // Create config object. Note that AppConfiguration could be null, then it would use default values
        public AppConfiguration Configuration => _appConfig.Get(() => new AppConfiguration(AppConfiguration, Log));
        private readonly GetOnce<AppConfiguration> _appConfig = new GetOnce<AppConfiguration>();

        private dynamic MakeDynProperty(IEntity contents, bool strict)
        {
            var wrapped = CmsEditDecorator.Wrap(contents, false);
            // TODO: FIGURE OUT IF STRICT
            return _cdfLazy.Value.AsDynamic(wrapped, strict: false);
        }

        internal void SetupAsConverter(CodeDataFactory cdf) => _cdfLazy.Inject(cdf);

        /// <inheritdoc cref="IDynamicCode12.Settings" />
        public dynamic Settings => AppSettings == null ? null : _settings.Get(() => MakeDynProperty(AppSettings, strict: false));
        private readonly GetOnce<dynamic> _settings = new GetOnce<dynamic>();

        /// <inheritdoc cref="IDynamicCode12.Resources" />
        public dynamic Resources => AppResources == null ? null : _res.Get(() => MakeDynProperty(AppResources, strict: false));
        private readonly GetOnce<dynamic> _res = new GetOnce<dynamic>();

        #endregion


    }
}