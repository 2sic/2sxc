using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sys.Caching.PiggyBack;

namespace ToSic.Sxc.Apps.Sys.Work;

/// <summary>
/// Service to figure out what views could be switched to, as the existing data should be compatible with the switched view.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkAppViewSwitch(WorkViewsGet viewsGet)
    : ServiceWithSetup<IAppWorkContext>("Cms.ViewRd", connect: [viewsGet])
{
    /// <summary>
    /// Helper record to get information about views, especially for selecting them based on the url identifier
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="UrlIdentifier"></param>
    /// <param name="IsRegex"></param>
    /// <param name="MainKey"></param>
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public record ViewInfoForPathSelect(IView View, string Name, string UrlIdentifier, bool IsRegex, string MainKey);

    /// <summary>
    /// Get all views which have a url identifier, to be used for view-switching
    /// </summary>
    /// <returns></returns>
    public List<ViewInfoForPathSelect> GetForViewSwitch()
    {
        var l = Log.Fn<List<ViewInfoForPathSelect>>();

        var allIViews = viewsGet.AllViews;

        // get from cache if available or generate
        var views = MyOptions.AppReader
            .GetCache()
            .PiggyBackGetExpiring(() => allIViews
                .Where(t => !string.IsNullOrEmpty(t.UrlIdentifier))
                .Select(v =>
                {
                    var urlIdentifier = v.UrlIdentifier.ToLowerInvariant();
                    var isRegex = urlIdentifier.EndsWith("/.*");
                    var mainParam = isRegex
                        ? urlIdentifier.Substring(0, urlIdentifier.Length - 3)
                        : urlIdentifier;

                    // Only save the necessary information in the PiggyBack
                    // Never save the View or the ViewInfoForPathSelect, as that would also preserve an old Service used in the View
                    return new
                    {
                        v.Entity,
                        v.Name,
                        urlIdentifier,
                        isRegex,
                        MainParam = mainParam.ToLowerInvariant()
                    };
                })
                .ToList()
            );

        var final = views.Value
            .Select(v => new ViewInfoForPathSelect(
                viewsGet.ViewOfEntity(v.Entity, v.Entity.EntityId, withServices: true, isReplacement: true),
                v.Name, v.urlIdentifier, v.isRegex, v.MainParam)
            )
            .ToList();

        return l.Return(final, $"all: {allIViews.Count}; switchable: {final.Count}; wasCached: {views.IsCached}");
    }

}