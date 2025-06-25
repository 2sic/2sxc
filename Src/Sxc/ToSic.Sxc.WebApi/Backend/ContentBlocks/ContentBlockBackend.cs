using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Sys;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Backend.InPage;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Cms.Internal.Publishing;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Internal.PageFeatures;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.ContentBlocks;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentBlockBackend(
    GenWorkPlus<WorkViews> workViews,
    Generator<MultiPermissionsApp> multiPermissionsApp,
    IPagePublishing publishing,
    GenWorkDb<WorkBlocksMod> workBlocksMod,
    ISxcCurrentContextService ctxService,
    LazySvc<IBlockResourceExtractor> optimizerLazy,
    LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
    AppWorkContextService appWorkCtxService,
    Generator<BlockOfEntity> entityBlockGenerator,
    Generator<IBlockBuilder> blockBuilderGenerator)
    : ServiceBase("Bck.FldLst",
        connect: [workViews, multiPermissionsApp, publishing, workBlocksMod, ctxService, optimizerLazy, blockEditorSelectorLazy, appWorkCtxService, entityBlockGenerator, blockBuilderGenerator])
{

    public IRenderResult NewBlockAndRender(int parentId, string field, int index, string app = "", Guid? guid = null)
    {
        var block = ctxService.BlockRequired();
        var appWorkCtxDb = appWorkCtxService.CtxWithDb(block.App);
        var entityId = workBlocksMod.New(appWorkCtxDb).NewBlockReference(parentId, field, index, app, guid);

        // now return a rendered instance
        var newContentBlock = entityBlockGenerator.New().GetBlockOfEntity(block, null, entityId);
        var builder = blockBuilderGenerator.New().Setup(newContentBlock);
        return builder.Run(true, specs: new());
    }

    public void AddItem(int? index = null)
    {
        Log.A($"add order:{index}");

        var block = ctxService.BlockRequired();
        var appWorCtxDb = appWorkCtxService.CtxWithDb(block.App);

        // use dnn versioning - this is always part of page
        publishing.DoInsidePublishing(block.Context, _ 
            => workBlocksMod.New(appWorCtxDb).AddEmptyItem(block.Configuration, index, block.Context.Publishing.ForceDraft));
    }

        
    public bool PublishPart(string part, int index)
    {
        Log.A($"try to publish #{index} on '{part}'");
        var block = ctxService.BlockRequired();
        ApiForBlockHelpers.ThrowIfNotAllowedInApp(multiPermissionsApp, block.Context, GrantSets.WritePublished);
        return blockEditorSelectorLazy.Value
            .GetEditor(block)
            .Publish(part, index);
    }

    public AjaxRenderDto RenderForAjax(int templateId, string lang, string root, string edition)
    {
        var l = Log.Fn<AjaxRenderDto>();
        l.A("1. Get Render result");
        var result = RenderToResult(templateId, lang, edition);

        l.A("2.1. Build Resources");
        var resources = new List<AjaxResourceDto>();
        var ver = EavSystemInfo.VersionWithStartUpBuild;
        if (result.Features?.Contains(SxcPageFeatures.TurnOn) == true)
            resources.Add(new() { Url = UrlHelpers.QuickAddUrlParameter(root.SuffixSlash() + SxcPageFeatures.TurnOn.UrlInDist, "v", ver) });

        l.A("2.2. Add JS & CSS which were stripped before");
        resources.AddRange(result.Assets
                               ?.Select(asset => new AjaxResourceDto
                               {
                                   // Note: Url can be empty if it has contents
                                   Url = string.IsNullOrWhiteSpace(asset.Url)
                                       ? null
                                       : UrlHelpers.QuickAddUrlParameter(asset.Url!, "v", ver),
                                   Type = asset.IsJs ? "js" : "css",
                                   Contents = asset.Content,
                                   Attributes = asset.HtmlAttributes,
                               })
                           ?? []
        );

        l.A("3. Add manual resources (fancybox etc.)");
        // First get all the parts out of HTML, as the configuration is still stored as plain HTML
        var mergedFeatures  = string.Join("\n", (result.FeaturesFromSettings?.Select(mc => mc.Html) ?? []));

        l.A("4.1. Process optimizers");
        var renderResult = optimizerLazy.Value.Process(mergedFeatures, new(extractAll: true));
        var rest = renderResult.Html;
        if (!string.IsNullOrWhiteSpace(rest)) 
            l.A("Warning: Rest after extraction should be empty - not handled ATM");

        l.A("4.2. Add more resources based on processed");
        resources.AddRange(renderResult.Assets.Select(asset => new AjaxResourceDto
        {
            Url = asset.Url,
            Type = asset.IsJs ? "js" : "css",
            Attributes = asset.HtmlAttributes,
        }));

        return l.ReturnAsOk(new()
        {
            Html = result.Html,
            Resources = resources
        });
    }

    private IRenderResult RenderToResult(int templateId, string lang, string edition)
    {
        var l = Log.Fn<IRenderResult>($"{nameof(templateId)}:{templateId}, {nameof(lang)}:{lang}");

        var block = ctxService.BlockRequired();
        // if a preview templateId was specified, swap to that
        if (templateId > 0)
        {
            var appWorkCtxPlus = appWorkCtxService.ContextPlus(block.Context.AppReaderRequired);
            var template = workViews.New(appWorkCtxPlus).Get(templateId);
            template.Edition = edition;
            block = ctxService.SwapBlockView(template);
        }

        var builder = blockBuilderGenerator.New().Setup(block);
        var result = builder.Run(true, specs: new());
        return l.ReturnAsOk(result);
    }

}