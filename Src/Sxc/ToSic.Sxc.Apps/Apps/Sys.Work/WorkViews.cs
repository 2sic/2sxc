using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sys.Caching.PiggyBack;

// note: not sure if the final namespace should be Sxc.Apps or Sxc.Views
namespace ToSic.Sxc.Apps.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkViews(GenWorkPlus<WorkEntities> appEntities, IZoneCultureResolver cultureResolver, Generator<QueryDefinitionFactory> qDefBuilder)
    : WorkUnitBase<IAppWorkCtxPlus>("Cms.ViewRd",
        connect: [appEntities, cultureResolver, qDefBuilder])
{
    /// <summary>
    /// Helper class to get information about views, especially for selecting them based on the url identifier
    /// </summary>
    /// <param name="View"></param>
    /// <param name="Name"></param>
    /// <param name="UrlIdentifier"></param>
    /// <param name="IsRegex"></param>
    /// <param name="MainKey"></param>
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public record ViewInfoForPathSelect(IView View, string Name, string UrlIdentifier, bool IsRegex, string MainKey);

    private List<IEntity> ViewEntities => field
        ??= AppWorkCtx.AppReader
            .GetCache()
            .PiggyBackGetExpiring(() => appEntities
                .New(AppWorkCtx)
                .Get(AppConstants.TemplateContentType)
                .ToList()
            ).Value;

    /// <summary>
    /// Get all the views.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Never cache this result in PiggyBack, as it has a service which would expire later on.
    /// </remarks>
    public List<IView> GetAll() =>
        _all ??= [.. ViewEntities
            .Select(e => ViewOfEntity(e, e.EntityId))
            .OrderBy(e => e.Name)];

    private List<IView>? _all;

    /// <summary>
    /// Get all views which have a url identifier, to be used for view-switching
    /// </summary>
    /// <returns></returns>
    public List<ViewInfoForPathSelect> GetForViewSwitch()
    {
        var l = Log.Fn<List<ViewInfoForPathSelect>>();

        // get from cache if available or generate
        var views = AppWorkCtx.AppReader
            .GetCache()
            .PiggyBackGetExpiring(() => GetAll()
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
                ViewOfEntity(v.Entity, v.Entity.EntityId, withServices: true, isReplacement: true),
                v.Name, v.urlIdentifier, v.isRegex, v.MainParam)
            )
            .ToList();

        return l.Return(final, $"all: {GetAll().Count}; switchable: {final.Count}; wasCached: {views.IsCached}");
    }


    public IView Get(int templateId)
        => ViewOfEntity(ViewEntities.GetOne(templateId), templateId, withServices: true);

    public IView Get(Guid guid)
        => ViewOfEntity(ViewEntities.GetOne(guid), guid, withServices: true);

    public IView Recreate(IView originalWithoutServices) => 
           ViewOfEntity(originalWithoutServices.Entity, originalWithoutServices.Id, withServices: true);

    private IView ViewOfEntity(IEntity? templateEntity, object templateId, bool withServices = true, bool isReplacement = false)
        => templateEntity == null
            ? throw new("The template with id '" + templateId + "' does not exist.")
            : new View(templateEntity, [cultureResolver.CurrentCultureCode], withServices ? qDefBuilder : null, isReplaced: isReplacement);

}