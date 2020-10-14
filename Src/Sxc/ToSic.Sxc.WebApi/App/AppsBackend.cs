using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.WebApi.App
{
    public class AppsBackend: WebApiBackendBase<AppsBackend>
    {
        public AppsBackend() : base("Bck.Apps")
        {
        }

        public List<AppDto> Apps(ITenant tenant, IBlock block, int zoneId)
        {
            var cms = new CmsZones(zoneId, Log);
            var configurationBuilder = ConfigurationProvider.Build(block, true);
            var list = cms.AppsRt.GetApps(tenant, configurationBuilder);
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
