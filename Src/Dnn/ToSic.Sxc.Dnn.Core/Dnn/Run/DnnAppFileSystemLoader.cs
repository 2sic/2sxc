using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// Special replacement of the FileSystemLoader - makes sure that unreliable Dnn initialization happens if necessary
    /// </summary>
    public class DnnAppFileSystemLoader : AppFileSystemLoader
    {
        /// <summary>
        /// Constructor for DI - you must always call Init(...) afterwards
        /// </summary>
        public DnnAppFileSystemLoader(IZoneMapper zoneMapper, Dependencies deps): base(deps, "Dnn.AppStf") 
            => ZoneMapper = zoneMapper;

        protected readonly IZoneMapper ZoneMapper;


        /// <summary>
        /// Init Path After AppId must be in an own method, as each implementation may have something custom to handle this
        /// </summary>
        /// <returns></returns>
        protected override bool InitPathAfterAppId()
        {
            var wrapLog = Log.Call<bool>();

            try
            {
                Log.A($"Trying to build path based on tenant. If it's in search mode, the {nameof(ISite)} would be {Eav.Constants.NullId}. Id: {Site.Id}");
                EnsureDnnSiteIsLoadedWhenDiFails();
                base.InitPathAfterAppId();
                return wrapLog(Path, true);
            }
            catch (Exception e)
            {
                // ignore
                Log.Exception(e);
                return wrapLog("error", false);
            }
        }


        /// <summary>
        /// Special workaround for DNN because the site information is often incomplete (buggy)
        /// </summary>
        /// <returns></returns>
        private bool EnsureDnnSiteIsLoadedWhenDiFails()
        {
            var wrapLog = Log.Call<bool>();
            if (Site.Id != Eav.Constants.NullId/* && Site.Id != 0*/) // 2021-12-09 2dm disabled zero check, because portal 0 is actually very common
                return wrapLog($"All ok since siteId isn't {Eav.Constants.NullId}", true);
            Log.A($"SiteId = {Site.Id} - not found. Must be in search mode or something else DI-style failed, will try to find correct PortalSettings");
            ZoneMapper.Init(Log);
            Site = ZoneMapper.SiteOfApp(AppId);
            return wrapLog($"SiteId: {Site.Id}", true);
        }

    }
}
