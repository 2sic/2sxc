using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
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
        protected override bool InitPathAfterAppId(string path)
        {
            var wrapLog = Log.Call<bool>(path);

            try
            {
                Log.Add($"Trying to build path based on tenant. If it's in search mode, the {nameof(ISite)} will be missing. Id: {Site.Id}");
                EnsureDnnSiteIsLoadedWhenDiFails();
                base.InitPathAfterAppId(path);
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
            if (Site.Id != Eav.Constants.NullId && Site.Id != 0)
                return wrapLog($"Stopped because siteId isn't zero or {Eav.Constants.NullId}", false);
            Log.Add("TenantId not found. Must be in search mode or something else DI-style failed, will try to find correct portalsettings");
            ZoneMapper.Init(Log);
            Site = ZoneMapper.SiteOfApp(AppId);
            return wrapLog("done", true);
        }

    }
}
