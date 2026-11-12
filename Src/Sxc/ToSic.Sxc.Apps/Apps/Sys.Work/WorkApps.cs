using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Apps.Sys.Ui;

namespace ToSic.Sxc.Apps.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkApps(IAppReaderFactory appReaders, Generator<IAppPathsMicroSvc> appPathsGen, LazySvc<GlobalPaths> globalPaths, IAppsCatalog appsCatalog)
    : ServiceBase("Cms.AppsRt", connect: [appReaders, appPathsGen, globalPaths, appsCatalog])
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


}