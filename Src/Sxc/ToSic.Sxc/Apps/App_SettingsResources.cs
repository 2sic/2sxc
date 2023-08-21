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

        private DynamicEntity MakeDynProperty(IEntity contents, bool propsRequired)
        {
            var wrapped = CmsEditDecorator.Wrap(contents, false);
            return _cdfLazy.Value.AsDynamic(wrapped, propsRequired: propsRequired);
        }

        internal void SetupAsConverter(CodeDataFactory cdf) => _cdfLazy.Inject(cdf);

        /// <inheritdoc cref="IDynamicCode12.Settings" />
        public dynamic Settings => AppSettings == null ? null : _settings.Get(() => MakeDynProperty(AppSettings, propsRequired: false));
        private readonly GetOnce<dynamic> _settings = new GetOnce<dynamic>();

        /// <inheritdoc cref="IDynamicCode12.Resources" />
        public dynamic Resources => AppResources == null ? null : _res.Get(() => MakeDynProperty(AppResources, propsRequired: false));
        private readonly GetOnce<dynamic> _res = new GetOnce<dynamic>();

        #endregion


    }
}