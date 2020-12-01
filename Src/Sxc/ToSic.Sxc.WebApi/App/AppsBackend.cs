using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.WebApi.App
{
    public class AppsBackend: WebApiBackendBase<AppsBackend>
    {
        private readonly CmsZones _cmsZones;

        public AppsBackend(CmsZones cmsZones, IServiceProvider serviceProvider) : base(serviceProvider, "Bck.Apps")
        {
            _cmsZones = cmsZones;
        }

        public List<AppDto> Apps(IContextOfApp context)
        {
            var cms = _cmsZones.Init(context.Site.ZoneId, Log);
            var configurationBuilder = ServiceProvider.Build<AppConfigDelegate>().Init(Log).Build(context.UserMayEdit);
            var list = cms.AppsRt.GetApps(context.Site, configurationBuilder);
            return list.Select(a => new AppDto
            {
                Id = a.AppId,
                IsApp = a.AppGuid != Eav.Constants.DefaultAppName,
                Guid = a.AppGuid,
                Name = a.Name,
                Folder = a.Folder,
                AppRoot = a.Path,
                IsHidden = a.Hidden,
                ConfigurationId = a.Configuration?.Id,
                Items = a.Data.Immutable.Count,
                Thumbnail = a.Thumbnail,
                Version = a.VersionSafe()
            }).ToList();
        }

    }
}
