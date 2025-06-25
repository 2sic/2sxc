using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Metadata.Sys;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;

// note: not sure if the final namespace should be Sxc.Apps or Sxc.Views
namespace ToSic.Sxc.Blocks.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkBlockViewsGet(GenWorkPlus<WorkViews> workViews, LazySvc<AppIconHelpers> appIconHelpers)
    : WorkUnitBase<IAppWorkCtxPlus>("Cms.ViewRd", connect: [workViews, appIconHelpers])
{
    [field: AllowNull, MaybeNull]
    public List<IView> GetAll => field
        ??= workViews.New(AppWorkCtx)
            .GetAll()
            .ToList();

    internal IEnumerable<TemplateUiInfo> GetCompatibleViews(IBlock block)
    {
        List<IView> availableTemplates;
        var blockConfiguration = block.Configuration;
        var items = blockConfiguration.Content;

        // if any items were already initialized...
        if (items.Any(e => e != null))
            availableTemplates = GetFullyCompatibleViews(blockConfiguration);

        // if it's only nulls, and only one (no list yet)
        else if (items.Count <= 1)
            availableTemplates = GetAll; 

        // if it's a list of nulls, only allow lists
        else
            availableTemplates = GetAll.Where(p => p.UseForList).ToList();

        var thumbnailHelper = appIconHelpers.Value;

        var result = availableTemplates
            .Select(t => new TemplateUiInfo
            {
                TemplateId = t.Id,
                Name = t.Name,
                ContentTypeStaticName = t.ContentType,
                IsHidden = t.IsHidden,
                Thumbnail = block.AppOrNull == null ? null : thumbnailHelper.IconPathOrNull(block.App, t, PathTypes.Link),
                IsDefault = t.Metadata.HasType(KnownDecorators.IsDefaultDecorator),
            })
            .ToList();
        return result;
    }


    /// <summary>
    /// Get templates which match the signature of possible content-items, presentation etc. of the current template
    /// </summary>
    /// <param name="blockConfig"></param>
    /// <returns></returns>
    private List<IView> GetFullyCompatibleViews(BlockConfiguration blockConfig)
    {
        var isList = blockConfig.Content.Count > 1;

        var compatibleTemplates = GetAll
            .Where(t => t.UseForList || !isList)
            .ToList();

        // Compatibility check must verify each config on the view and compare with the current blockConfig
        compatibleTemplates = compatibleTemplates
            .Where(t => VerifyCompatible(blockConfig.Content, t.ContentType))
            .Where(t => VerifyCompatible(blockConfig.Presentation, t.PresentationType))
            .Where(t => VerifyCompatible(blockConfig.Header, t.HeaderType))
            .Where(t => VerifyCompatible(blockConfig.HeaderPresentation, t.HeaderPresentationType))
            .ToList();

        return compatibleTemplates;

        bool VerifyCompatible(IList<IEntity?> list, string expectedName)
        {
            var firstNonNull = list.FirstOrDefault(e => e != null);
            if (firstNonNull == null)
                return true; // all nulls, so compatible
            return firstNonNull.Type.NameId == expectedName;
        }
    }


}