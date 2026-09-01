using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Context;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Apps.Sys.Ui;

namespace ToSic.Sxc.Apps.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkApps(IAppStateCacheService appStates, IAppReaderFactory appReaders, Generator<IAppPathsMicroSvc> appPathsGen, LazySvc<GlobalPaths> globalPaths, IAppsCatalog appsCatalog)
    : ServiceBase("Cms.AppsRt", connect: [appStates, appReaders, appPathsGen, globalPaths, appsCatalog])
{

    public IList<AppUiInfo> GetSelectableApps(ISite site, string? filter)
    {
        var l = Log.Fn<List<AppUiInfo>>($"filter:{filter}");
        var list =
            GetApps(site)
                .Where(reader =>
                {
                    var name = reader.Specs.Name;
                    return name != KnownAppsConstants.ContentAppName
                           && name != KnownAppsConstants.ErrorAppName // "Error" it is a name of empty Content app (before content templates are installed)
                           && name != KnownAppsConstants.PrimaryAppName
                           && name != KnownAppsConstants.PrimaryAppGuid;
                }) // #SiteApp v13
                .Where(reader => !reader.Specs.Configuration.IsHidden)
                .Select(reader =>
                {
                    var paths = appPathsGen.New().Get(reader, site);
                    var thumbnail = AppAssetThumbnail.GetUrl(reader, paths, globalPaths);
                    var specs = reader.Specs;
                    return new AppUiInfo
                    {
                        Name = specs.Name,
                        AppId = specs.AppId,
                        SupportsAjaxReload = specs.Configuration.EnableAjax,
                        Thumbnail = thumbnail,
                        Version = specs.Configuration.Version.ToString() ?? ""
                    };
                })
                .ToList();

        if (string.IsNullOrWhiteSpace(filter)) return l.Return(list, "unfiltered");

        // New feature in 10.27 - if app-list is provided, only return these
        var appNames = filter.CsvToArrayWithoutEmpty();
        list = list.Where(ap => appNames
                .Any(name => string.Equals(name, ap.Name, StringComparison.InvariantCultureIgnoreCase)))
            .ToList();
        return l.Return(list, "ok");
    }

    /// <summary>
    /// Returns all Apps for the current zone
    /// </summary>
    /// <returns></returns>
    public List<IAppReader> GetApps(ISite site)
    {
        // todo: unclear if this is the right way to do this - probably the ZoneId should come from the site?
        var zId = site.ZoneId;
        var appIds = appsCatalog.Apps(zId);

        return appIds
            .Select(a => appReaders.Get(new AppIdentityPure(zId, a.Key)))
            .OrderBy(a => a.Specs.Name)
            .ToList();
    }

    /// <summary>
    /// Returns all Apps for the current zone
    /// </summary>
    /// <returns></returns>
    public ICollection<IAppReader> GetInheritableApps(ISite site)
    {
        var l = Log.Fn<ICollection<IAppReader>>();
        
        var defaultAppId = appsCatalog.DefaultAppIdentity(site.ZoneId).AppId;

        // Get existing apps, as we should not list inheritable apps which are already inherited
        var siteApps = appsCatalog.Apps(site.ZoneId)
            .Select(a => appReaders.Get(new AppIdentityPure(site.ZoneId, a.Key)))
            // Content is registered by default; only block inheritance once it has real data.
            .Where(appReader => appReader.AppId != defaultAppId || appReader.List.Count > 3)
            .Select(a => a.Specs.Folder)
            .ToListOpt();

        var zones = appsCatalog.Zones;
        var result = zones
            // Skip all global apps on the current site, as they shouldn't be inheritable in this site
            .Where(z => z.Key != site.ZoneId)
            .SelectMany(zSet =>
            {
                var zId = zSet.Key;
                var appIds = appsCatalog.Apps(zId);

                return appIds
                    .Select(a => new AppIdentityPure(zId, a.Key))
                    // Skip all which are not yet in memory - not perfect
                    // ...but otherwise it can take very long after a restart, especially with hundreds of sites
                    // Means that for us to be able to find a master app
                    // that apps-management must have been accessed at least once after the restart, so that the app is in memory
                    .Where(appStates.IsCached)
                    .Select(appReaders.Get)
                    .Where(reader =>
                        // Only show apps which are shared
                        reader.IsShared()
                        // and not already in the current site (with the same folder name)
                        && !siteApps.Any(sa => sa.EqualsInsensitive(reader.Specs.Folder)))
                    .OrderBy(reader => reader.Specs.Name)
                    .ToListOpt();
            })
            .ToListOpt();
        
        return l.Return(result);
    }

}
