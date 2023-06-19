using System;
using ToSic.Eav.CodeChanges;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data.AsConverter;
using ToSic.Sxc.Services.CmsService;

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

        private dynamic MakeDynProperty(IEntity contents)
        {
            var wrapped = CmsEditDecorator.Wrap(contents, false);
            return (_asc ?? _asConverter.Value).AsDynamic(wrapped);
        }

        internal void AddDynamicEntityServices(AsConverterService asc)
        {
            _asc = asc;
        }
        private AsConverterService _asc;

        /// <inheritdoc />
        public dynamic Settings => AppSettings == null ? null : _settings.Get(() => MakeDynProperty(AppSettings));
        private readonly GetOnce<dynamic> _settings = new GetOnce<dynamic>();

        /// <inheritdoc />
        public dynamic Resources => AppResources == null ? null : _res.Get(() => MakeDynProperty(AppResources));
        private readonly GetOnce<dynamic> _res = new GetOnce<dynamic>();

        #endregion


    }
}