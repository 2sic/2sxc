using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Internal.MetadataDecorators;
using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Apps.Internal.Work;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class WorkApps(IAppStateCacheService appStates, IAppReaderFactory appReaders, Generator<IAppPathsMicroSvc> appPathsGen, LazySvc<GlobalPaths> globalPaths, IAppsCatalog appsCatalog)
    : ServiceBase("Cms.AppsRt", connect: [appStates, appReaders, appPathsGen, globalPaths, appsCatalog])
{

    public IList<AppUiInfo> GetSelectableApps(ISite site, string filter)
    {
        var l = Log.Fn<List<AppUiInfo>>($"filter:{filter}");
        var list =
            GetApps(site)
                .Where(reader =>
                {
                    var name = reader.Specs.Name;
                    return name != Eav.Constants.ContentAppName
                           && name != Eav.Constants.ErrorAppName // "Error" it is a name of empty Content app (before content templates are installed)
                           && name != Eav.Constants.PrimaryAppName
                           && name != Eav.Constants.PrimaryAppGuid;
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
                        SupportsAjaxReload = specs.Configuration?.EnableAjax ?? false,
                        Thumbnail = thumbnail,
                        Version = specs.Configuration?.Version?.ToString() ?? ""
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
    public List<IAppReader> GetInheritableApps(ISite site)
    {
        // Get existing apps, as we should not list inheritable apps which are already inherited
        var siteApps = appsCatalog.Apps(site.ZoneId)
            // TODO: #AppStates we could only get the specs here...
            .Select(a => appReaders.Get(a.Key).Specs.Folder)
            .ToList();

        var zones = appsCatalog.Zones;
        var result = zones
            // Skip all global apps on the current site, as they shouldn't be inheritable
            .Where(z => z.Key != site.ZoneId)
            .SelectMany(zSet =>
            {
                // todo: probably the ZoneId should come from the site?
                var zId = zSet.Key;
                var appIds = appsCatalog.Apps(zId);

                return appIds
                    //.Select(a => new AppIdentityPure(zId, a.Key))
                    .Select(a =>
                    {
                        var appIdentity = new AppIdentityPure(zId, a.Key);
                        return appStates.IsCached(appIdentity)
                            ? appReaders.Get(appIdentity)
                            : null;
                    })
                    .Where(reader =>
                    {
                        if (reader == null) return false;
                        // if (!appStateWithCacheInfo.IsCached(aId)) return false;
                        //var appState = _appStates.GetReader(aId);
                        return reader?.IsShared() == true && !siteApps.Any(sa => sa.Equals(reader.Specs.Folder, StringComparison.InvariantCultureIgnoreCase));
                    })
                    //.Select(a => _appGenerator.New().PreInit(site).Init(a, buildConfig) as IApp)
                    .OrderBy(a => a.Specs.Name)
                    .ToList();
            })
            .ToList();
        return result;
    }

}