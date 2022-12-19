using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Apps;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Web.LightSpeed;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.App
{
    public class AppsBackend: WebApiBackendBase<AppsBackend>
    {
        private readonly CmsZones _cmsZones;
        private readonly IContextOfSite _context;

        public AppsBackend(CmsZones cmsZones, IContextOfSite context, IServiceProvider serviceProvider) : base(serviceProvider, "Bck.Apps")
        {
            _cmsZones = cmsZones;
            _context = context;
        }

        public List<AppDto> Apps()
        {
            var cms = _cmsZones.Init(Log).SetId(_context.Site.ZoneId);
            var configurationBuilder = GetService<AppConfigDelegate>().Init(Log).Build(_context.UserMayEdit);
            var list = cms.AppsRt.GetApps(_context.Site, configurationBuilder);
            return list.Select(CreateAppDto).ToList();
        }

        private static AppDto CreateAppDto(IApp a)
        {
            AppMetadataDto lightspeed = null;
            var lsEntity = a.AppState.Metadata.FirstOrDefaultOfType(LightSpeedDecorator.TypeName);
            if (lsEntity != null)
            {
                var lsd = new LightSpeedDecorator(lsEntity);
                lightspeed = new AppMetadataDto { Id = lsd.Id, Title = lsd.Title, IsEnabled = lsd.IsEnabled };
            }
            return new AppDto
            {
                Id = a.AppId,
                IsApp = a.NameId != Eav.Constants.DefaultAppGuid &&
                        a.NameId != Eav.Constants.PrimaryAppGuid, // #SiteApp v13
                Guid = a.NameId,
                Name = a.Name,
                Folder = a.Folder,
                AppRoot = a.Path,
                IsHidden = a.Hidden,
                ConfigurationId = a.Configuration?.Id,
                Items = a.Data.List.Count(),
                Thumbnail = a.Thumbnail,
                Version = a.VersionSafe(),
                IsGlobal = a.AppState.IsShared(),
                IsInherited = a.AppState.IsInherited(),
                Lightspeed = lightspeed,
            };
        }

        public List<AppDto> GetInheritableApps()
        {
            var cms = _cmsZones.Init(Log).SetId(_context.Site.ZoneId);
            var configurationBuilder = GetService<AppConfigDelegate>().Init(Log).Build(_context.UserMayEdit);
            var list = cms.AppsRt.GetInheritableApps(_context.Site, configurationBuilder);
            return list.Select(CreateAppDto).ToList();
        }
    }
}
