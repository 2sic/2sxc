using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.ContentTypes;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.Data.ValueConverter.Sys;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.Metadata.Sys;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;

// note: not sure if the final namespace should be Sxc.Apps or Sxc.Views
namespace ToSic.Sxc.Apps.Internal.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkViews(
    GenWorkPlus<WorkEntities> appEntities,
    LazySvc<IValueConverter> valConverterLazy,
    IZoneCultureResolver cultureResolver,
    IConvertToEavLight dataToFormatLight,
    Generator<QueryDefinitionBuilder> qDefBuilder)
    : WorkUnitBase<IAppWorkCtxPlus>("Cms.ViewRd",
        connect: [appEntities, valConverterLazy, cultureResolver, dataToFormatLight, qDefBuilder])
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

    private List<IEntity> ViewEntities => _viewDs.Get(() => AppWorkCtx.AppReader.GetPiggyBackPropExpiring(
            () => appEntities.New(AppWorkCtx)
                .Get(AppConstants.TemplateContentType)
                .ToList()
        ).Value
    )!;
    private readonly GetOnce<List<IEntity>> _viewDs = new();

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

    public IEnumerable<IView> GetRazor() => GetAll().Where(t => t.IsRazor);
    public IEnumerable<IView> GetToken() => GetAll().Where(t => !t.IsRazor);

    /// <summary>
    /// Get all views which have a url identifier, to be used for view-switching
    /// </summary>
    /// <returns></returns>
    public List<ViewInfoForPathSelect> GetForViewSwitch()
    {
        var l = Log.Fn<List<ViewInfoForPathSelect>>();

        // get from cache if available or generate
        var views = AppWorkCtx.AppReader.GetPiggyBackPropExpiring(() => GetAll()
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
        => ViewOfEntity(ViewEntities.One(templateId), templateId, withServices: true);

    public IView Get(Guid guid)
        => ViewOfEntity(ViewEntities.One(guid), guid, withServices: true);

    public IView Recreate(IView originalWithoutServices) => 
           ViewOfEntity(originalWithoutServices.Entity, originalWithoutServices.Id, withServices: true);

    private IView ViewOfEntity(IEntity? templateEntity, object templateId, bool withServices = true, bool isReplacement = false)
        => templateEntity == null
            ? throw new("The template with id '" + templateId + "' does not exist.")
            : new View(templateEntity, [cultureResolver.CurrentCultureCode], Log, withServices ? qDefBuilder : null, isReplaced: isReplacement);


    // todo: check if this call could be replaced with the normal ContentTypeController.Get to prevent redundant code
    public IList<ContentTypeUiInfo> GetContentTypesWithStatus(string appPath, string appPathShared)
    {
        var templates = GetAll().ToList();
        var visible = templates.Where(t => !t.IsHidden).ToList();

        var valConverter = valConverterLazy.Value;

        var result = AppWorkCtx.AppReader.ContentTypes
            .OfScope(ScopeConstants.Default) 
            .Where(ct => templates.Any(t => t.ContentType == ct.NameId)) // must exist in at least 1 template
            .OrderBy(ct => ct.Name)
            .Select(ct =>
            {
                var details = ct.DetailsOrNull();
                var thumbnail = valConverter.ToValue(details?.Icon);
                if (AppIconHelpers.HasAppPathToken(thumbnail))
                    thumbnail = AppIconHelpers.AppPathTokenReplace(thumbnail, appPath, appPathShared);
                return new ContentTypeUiInfo {
                    StaticName = ct.NameId,
                    Name = ct.Name,
                    IsHidden = visible.All(t => t.ContentType != ct.NameId),   // must check if *any* template is visible, otherwise tell the UI that it's hidden
                    Thumbnail = thumbnail,
                    Properties = details?.Entity == null
                        ? null
                        : dataToFormatLight.Convert(details.Entity),
                    IsDefault = ct.Metadata.HasType(KnownDecorators.IsDefaultDecorator),
                };
            })
            .ToList();
        return result;
    }
}