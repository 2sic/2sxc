using ToSic.Eav.Apps.Views.Sys;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

// note: not sure if the final namespace should be Sxc.Apps or Sxc.Views
namespace ToSic.Sxc.Apps.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkViewsGet(AppWorkChain<WorkEntities> appEntities, IZoneCultureResolver cultureResolver, Generator<QueryDefinitionFactory> qDefBuilder)
    : ServiceWithSetup<IAppWorkContext>("Cms.ViewRd", connect: [appEntities, cultureResolver, qDefBuilder])
{
    private List<IEntity> ViewEntities => field ??= appEntities.New(MyOptions).GetAllViewEntities(MyOptions.AppReader);

    /// <summary>
    /// Get all the views.
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// Never cache this result in PiggyBack, as it has a service which would expire later on.
    /// </remarks>
    public List<IView> AllViews => field ??=
    [
        .. ViewEntities
            .Select(e => ViewOfEntity(e, e.EntityId))
            .OrderBy(e => e.Name)
    ];


    public IView Get(int templateId)
        => ViewOfEntity(ViewEntities.GetOne(templateId), templateId, withServices: true);

    public IView Get(Guid guid)
        => ViewOfEntity(ViewEntities.GetOne(guid), guid, withServices: true);

    internal IView ViewOfEntity(IEntity? templateEntity, object templateId, bool withServices = true, bool isReplacement = false)
        => templateEntity == null
            ? throw new("The template with id '" + templateId + "' does not exist.")
            : new View(templateEntity, [cultureResolver.CurrentCultureCode], withServices ? qDefBuilder : null, isReplaced: isReplacement);

}