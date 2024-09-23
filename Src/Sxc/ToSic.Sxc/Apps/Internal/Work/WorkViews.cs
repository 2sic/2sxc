using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Metadata;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;

// note: not sure if the final namespace should be Sxc.Apps or Sxc.Views
namespace ToSic.Sxc.Apps.Internal.Work;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class WorkViews(
    GenWorkPlus<WorkEntities> appEntities,
    LazySvc<IValueConverter> valConverterLazy,
    IZoneCultureResolver cultureResolver,
    IConvertToEavLight dataToFormatLight,
    LazySvc<AppIconHelpers> appIconHelpers,
    Generator<QueryDefinitionBuilder> qDefBuilder)
    : WorkUnitBase<IAppWorkCtxPlus>("Cms.ViewRd",
        connect: [appEntities, valConverterLazy, cultureResolver, dataToFormatLight, appIconHelpers, qDefBuilder])
{
    /// <summary>
    /// Helper class to get information about views, especially for selecting them based on the url identifier
    /// </summary>
    /// <param name="View"></param>
    /// <param name="Name"></param>
    /// <param name="UrlIdentifier"></param>
    /// <param name="IsRegex"></param>
    /// <param name="MainKey"></param>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public record ViewInfoForPathSelect(IView View, string Name, string UrlIdentifier, bool IsRegex, string MainKey);

    private List<IEntity> ViewEntities => _viewDs.Get(() => AppWorkCtx.AppReader.GetPiggyBackPropExpiring(
            () => appEntities.New(AppWorkCtx)
                .Get(AppConstants.TemplateContentType)
                .ToList()
        ).Value
    );
    private readonly GetOnce<List<IEntity>> _viewDs = new();

    /// <summary>
    /// Get all the views.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Never cache this result in PiggyBack, as it has a service which would expire later on.
    /// </remarks>
    public IList<IView> GetAll() =>
        _all ??= [.. ViewEntities
            .Select(e => ViewOfEntity(e, e.EntityId))
            .OrderBy(e => e.Name)];

    private IList<IView> _all;

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


    public IView Get(int templateId) => ViewOfEntity(ViewEntities.One(templateId), templateId, withServices: true);

    public IView Get(Guid guid) => ViewOfEntity(ViewEntities.One(guid), guid, withServices: true);

    public IView Recreate(IView originalWithoutServices) => 
           ViewOfEntity(originalWithoutServices.Entity, originalWithoutServices.Id, withServices: true);

    private IView ViewOfEntity(IEntity templateEntity, object templateId, bool withServices = true, bool isReplacement = false)
        => templateEntity == null
            ? throw new("The template with id '" + templateId + "' does not exist.")
            : new View(templateEntity, [cultureResolver.CurrentCultureCode], Log, withServices ? qDefBuilder : null, isReplaced: isReplacement);


    internal IEnumerable<TemplateUiInfo> GetCompatibleViews(IApp app, BlockConfiguration blockConfiguration)
    {
        IEnumerable<IView> availableTemplates;
        var items = blockConfiguration.Content;

        // if any items were already initialized...
        if (items.Any(e => e != null))
            availableTemplates = GetFullyCompatibleViews(blockConfiguration);

        // if it's only nulls, and only one (no list yet)
        else if (items.Count <= 1)
            availableTemplates = GetAll(); 

        // if it's a list of nulls, only allow lists
        else
            availableTemplates = GetAll().Where(p => p.UseForList);

        var thumbnailHelper = appIconHelpers.Value;

        var result = availableTemplates.Select(t => new TemplateUiInfo
        {
            TemplateId = t.Id,
            Name = t.Name,
            ContentTypeStaticName = t.ContentType,
            IsHidden = t.IsHidden,
            Thumbnail = thumbnailHelper.IconPathOrNull(app, t, PathTypes.Link),
            IsDefault = t.Metadata.HasType(Decorators.IsDefaultDecorator),
        });
        return result;
    }


    /// <summary>
    /// Get templates which match the signature of possible content-items, presentation etc. of the current template
    /// </summary>
    /// <param name="blockConfig"></param>
    /// <returns></returns>
    private IEnumerable<IView> GetFullyCompatibleViews(BlockConfiguration blockConfig)
    {
        var isList = blockConfig.Content.Count > 1;

        var compatibleTemplates = GetAll()
            .Where(t => t.UseForList || !isList);

        // Compatibility check must verify each config on the view and compare with the current blockConfig
        compatibleTemplates = compatibleTemplates
            .Where(t => blockConfig.Content.All(c => c == null) || blockConfig.Content.First(e => e != null).Type.NameId == t.ContentType)
            .Where(t => blockConfig.Presentation.All(c => c == null) || blockConfig.Presentation.First(e => e != null).Type.NameId == t.PresentationType)
            .Where(t => blockConfig.Header.All(c => c == null) || blockConfig.Header.First(e => e != null).Type.NameId == t.HeaderType)
            .Where(t => blockConfig.HeaderPresentation.All(c => c == null) || blockConfig.HeaderPresentation.First(e => e != null).Type.NameId == t.HeaderPresentationType);

        return compatibleTemplates;
    }


    // todo: check if this call could be replaced with the normal ContentTypeController.Get to prevent redundant code
    public IList<ContentTypeUiInfo> GetContentTypesWithStatus(string appPath, string appPathShared)
    {
        var templates = GetAll().ToList();
        var visible = templates.Where(t => !t.IsHidden).ToList();

        var valConverter = valConverterLazy.Value;

        var result = AppWorkCtx.AppReader.ContentTypes
            .OfScope(Scopes.Default) 
            .Where(ct => templates.Any(t => t.ContentType == ct.NameId)) // must exist in at least 1 template
            .OrderBy(ct => ct.Name)
            .Select(ct =>
            {
                var details = ct.Metadata.DetailsOrNull;
                var thumbnail = valConverter.ToValue(details?.Icon);
                if (AppIconHelpers.HasAppPathToken(thumbnail))
                    thumbnail = AppIconHelpers.AppPathTokenReplace(thumbnail, appPath, appPathShared);
                return new ContentTypeUiInfo {
                    StaticName = ct.NameId,
                    Name = ct.Name,
                    IsHidden = visible.All(t => t.ContentType != ct.NameId),   // must check if *any* template is visible, otherwise tell the UI that it's hidden
                    Thumbnail = thumbnail,
                    Properties = dataToFormatLight.Convert(details?.Entity),
                    IsDefault = ct.Metadata.HasType(Decorators.IsDefaultDecorator),
                };
            })
            .ToList();
        return result;
    }
}