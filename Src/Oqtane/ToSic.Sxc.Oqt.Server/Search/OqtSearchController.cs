using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Caching;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Render.Polymorphism.Sys;
using ToSic.Sxc.Search;
using ToSic.Sxc.Search.Sys;
using Log = ToSic.Sys.Logging.Log;

namespace ToSic.Sxc.Oqt.Server.Search;

internal class OqtSearchController(
    ISite site,
    ISiteRepository sites,
    IAppsCacheSelector appsCache,
    Generator<IModule> moduleGenerator,
    OqtContextOfBlockFactory contextFactory,
    Generator<BlockOfModule> blockGenerator,
    LazySvc<SearchIndexing> indexing,
    IEditionService editionService,
    LazySvc<IAppPathsMicroSvc> appPaths,
    ISxcCurrentContextService currentContext,
    LazySvc<ILogStore> logStore)
    : ServiceBase("Oqt.Search", connect: [site, appsCache, moduleGenerator, blockGenerator, indexing, editionService, appPaths, logStore])
{
    public List<SearchContent> GetSearchContents(PageModule pageModule, DateTime lastIndexedOn)
    {
        var l = Log.Fn<List<SearchContent>>($"module:{pageModule.ModuleId}, page:{pageModule.PageId}", timer: true);
        // Background scans can be frequent; retain an Insights entry only for a failed
        // scan, with the module/page identifiers needed to investigate it.
        var logWithPreserve = Log as Log;
        logWithPreserve?.Preserve = false;

        try
        {
            // The user-facing site service requires HttpContext and filters by the current
            // user. The background indexer reads the site through Oqtane's repository instead.
            var oqtSite = sites.GetSite(pageModule.Module.SiteId);
            ((OqtSite)site).Init(oqtSite);
            var module = ((OqtModule)moduleGenerator.New()).Init(pageModule.Module);
            var identity = module.BlockIdentifier;
            if (identity == null || identity.AppId is AppConstants.AppIdNotFound or EavConstants.NullId or KnownAppsConstants.AppIdEmpty)
                return l.Return([], "module has no app");

            // Note: the using-overload is probably net necessary in Oqtane, as the Site seems to be injectable
            // which doesn't work in DNN
            // probably an early access also doesn't matter - we can probably remove this
            // I assume it's just here because DNN had it - discuss TODO: @STV
            //using (Ctx<IZoneCultureResolver>.Use(new ZoneCultureInfo(site.DefaultCultureCode, site.DefaultCultureCode)))
            appsCache.Value.Get(identity.PureIdentity());
            var context = contextFactory.Create(pageModule.PageId, module);
            var block = blockGenerator.New().GetBlockOfModule(context);
            if (!block.ViewIsReady || block.View.SearchIndexingDisabled)
                return l.Return([], "module has no view or search is disabled");

            // Custom controllers and their queries see only this module's context.
            currentContext.ShareBlockSpecs(block);
            var language = string.IsNullOrWhiteSpace(oqtSite.CultureCode)
                ? site.DefaultCultureCode
                : OqtCulture.NormalizeCultureCode(oqtSite.CultureCode);
            var items = BuildItems(block, pageModule, language);
            l.A($"streams:{items.Count}, items:{items.Sum(s => s.Value.Count)}");
            if (items.Count == 0)
                return l.Return([], "no streams to index");

            if (!string.IsNullOrWhiteSpace(block.View.ViewController))
            {
                var paths = appPaths.Value.Get(block.Context.AppReaderRequired, site);
                var path = block.View.IsShared ? paths.RelativePathShared : paths.RelativePath;
                var controller = indexing.Value.CreateViewController(block, editionService.Edition(block), path);
                if (controller == null)
                {
                    // As on DNN, a configured class without ICustomizeSearch supplies no documents.
                    logWithPreserve?.Preserve = true;
                    return l.Return([], "view controller does not implement ICustomizeSearch");
                }

                controller.CustomizeSearch(items, block.Context.Module, lastIndexedOn);
            }

            // Match DNN's customization order: customize first, then select changed items.
            var results = OqtSearchContent.Map(items.Values.SelectMany(s => s), pageModule, lastIndexedOn);
            return l.Return(results, $"items:{results.Count}");
        }
        catch (Exception ex)
        {
            // Let the native job report the failure and retain its diagnostic context.
            logWithPreserve?.Preserve = true;
            l.Ex(ex);
            throw;
        }
        finally
        {
            if (logWithPreserve?.Preserve == true)
                logStore.Value.Add("oqtane-search", Log);
        }
    }

    private static Dictionary<string, List<ISearchItem>> BuildItems(IBlock block, PageModule pageModule, string language)
        => SearchIndexing.Streams(block.Data.Out, block.View.SearchIndexingStreams)
            .ToDictionary(stream => stream.Key, stream => stream.Value.List
                .Select(entity => (ISearchItem)OqtSearchContent.FromEntity(entity, stream.Key, language, pageModule))
                .ToList());
}
