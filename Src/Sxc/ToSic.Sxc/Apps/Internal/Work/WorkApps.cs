using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.Internal.MetadataDecorators;
using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Apps.Internal.Work;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class WorkApps : ServiceBase
{
    private readonly Generator<IAppPathsMicroSvc> _appPathsGen;
    private readonly LazySvc<GlobalPaths> _globalPaths;

    #region Constructor / DI

    public WorkApps(IAppStates appStates/*, Generator<App> appGenerator*/, Generator<IAppPathsMicroSvc> appPathsGen, LazySvc<GlobalPaths> globalPaths) : base("Cms.AppsRt")
    {
        ConnectServices(
            _appStates = appStates,
            _appPathsGen = appPathsGen,
            _globalPaths = globalPaths
            //_appGenerator = appGenerator
        );
    }

    private readonly IAppStates _appStates;
    //private readonly Generator<App> _appGenerator;

    #endregion

    public IList<AppUiInfo> GetSelectableApps(ISite site, string filter)
    {
        var l = Log.Fn<List<AppUiInfo>>($"filter:{filter}");
        var list =
            GetApps(site)
                .Where(a => a.Name != Eav.Constants.ContentAppName 
                            && a.Name != Eav.Constants.ErrorAppName // "Error" it is a name of empty Content app (before content templates are installed)
                            && a.Name != Eav.Constants.PrimaryAppName 
                            && a.Name != Eav.Constants.PrimaryAppGuid) // #SiteApp v13
                .Where(a => !a.Configuration.IsHidden)
        .Select(a =>
        {
                    var paths = _appPathsGen.New().Init(site, a);
                    var thumbnail = AppAssetThumbnail.GetUrl(a, paths, _globalPaths);
                    return new AppUiInfo
                    {
                        Name = a.Name,
                        AppId = a.AppId,
                        SupportsAjaxReload = a.Configuration?.EnableAjax ?? false,
                        Thumbnail = thumbnail, // a.Thumbnail,
                        Version = a.Configuration?.Version?.ToString() ?? ""
                    };
                })
                .ToList();

        if (string.IsNullOrWhiteSpace(filter)) return l.Return(list, "unfiltered");

        // New feature in 10.27 - if app-list is provided, only return these
        var appNames = filter.Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
        list = list.Where(ap => appNames
                .Any(name => string.Equals(name, ap.Name, StringComparison.InvariantCultureIgnoreCase)))
            .ToList();
        return l.Return(list, "ok");
    }

    /// <summary>
    /// Returns all Apps for the current zone
    /// </summary>
    /// <returns></returns>
    public List<IAppStateInternal> GetApps(ISite site)
    {
        // todo: unclear if this is the right way to do this - probably the ZoneId should come from the site?
        var zId = site.ZoneId;
        var appIds = _appStates.Apps(zId);

        return appIds
            .Select(a =>
            {
                var appIdentity = new AppIdentityPure(zId, a.Key);
                return _appStates.GetReader(appIdentity);
            })
            .OrderBy(a => a.Name)
            .ToList();

        //return appIds
        //    .Select(a => _appGenerator.New()
        //        .PreInit(site)
        //        .Init(new AppIdentityPure(zId, a.Key), buildConfig) as IApp)
        //    .OrderBy(a => a.Name)
        //    .ToList();
    }

    /// <summary>
    /// Returns all Apps for the current zone
    /// </summary>
    /// <returns></returns>
    public List<IAppStateInternal> GetInheritableApps(ISite site)
    {
        // Get existing apps, as we should not list inheritable apps which are already inherited
        var siteApps = _appStates.Apps(site.ZoneId)
            .Select(a => _appStates.GetReader(a.Key).Folder)
            .ToList();

        var zones = _appStates.Zones;
        var appStateWithCacheInfo = _appStates;
        var result = zones
            // Skip all global apps on the current site, as they shouldn't be inheritable
            .Where(z => z.Key != site.ZoneId)
            .SelectMany(zSet =>
            {
                // todo: probably the ZoneId should come from the site?
                var zId = zSet.Key;
                var appIds = _appStates.Apps(zId);

                return appIds
                    //.Select(a => new AppIdentityPure(zId, a.Key))
                    .Select(a =>
                    {
                        var appIdentity = new AppIdentityPure(zId, a.Key);
                        return appStateWithCacheInfo.IsCached(appIdentity) ? _appStates.GetReader(appIdentity) : null;
                    })
                    .Where(state =>
                    {
                        if (state == null) return false;
                        // if (!appStateWithCacheInfo.IsCached(aId)) return false;
                        //var appState = _appStates.GetReader(aId);
                        return state?.IsShared() == true && !siteApps.Any(sa => sa.Equals(state.Folder, StringComparison.InvariantCultureIgnoreCase));
                    })
                    //.Select(a => _appGenerator.New().PreInit(site).Init(a, buildConfig) as IApp)
                    .OrderBy(a => a.Name)
                    .ToList();
            })
            .ToList();
        return result;
    }

}