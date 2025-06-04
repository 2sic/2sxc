using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Metadata;
using ToSic.Eav.Metadata.Sys;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;

// note: not sure if the final namespace should be Sxc.Apps or Sxc.Views
namespace ToSic.Sxc.Blocks.Work.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkBlockViewsGet(GenWorkPlus<WorkViews> workViews, LazySvc<AppIconHelpers> appIconHelpers)
    : WorkUnitBase<IAppWorkCtxPlus>("Cms.ViewRd", connect: [workViews, appIconHelpers])
{
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
                Thumbnail = thumbnailHelper.IconPathOrNull(block?.App, t, PathTypes.Link),
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
            .Where(t => blockConfig.Content.All(c => c == null) || blockConfig.Content.First(e => e != null).Type.NameId == t.ContentType)
            .Where(t => blockConfig.Presentation.All(c => c == null) || blockConfig.Presentation.First(e => e != null).Type.NameId == t.PresentationType)
            .Where(t => blockConfig.Header.All(c => c == null) || blockConfig.Header.First(e => e != null).Type.NameId == t.HeaderType)
            .Where(t => blockConfig.HeaderPresentation.All(c => c == null) || blockConfig.HeaderPresentation.First(e => e != null).Type.NameId == t.HeaderPresentationType)
            .ToList();

        return compatibleTemplates;
    }


}