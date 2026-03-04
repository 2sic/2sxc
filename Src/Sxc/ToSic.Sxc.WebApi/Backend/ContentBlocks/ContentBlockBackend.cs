using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.InPage;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockEditor;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.Output;
using ToSic.Sxc.Render.Sys.RenderBlock;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.ContentBlocks;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentBlockBackend(
    GenWorkPlus<WorkViews> workViews,
    Generator<MultiPermissionsApp, MultiPermissionsApp.Options> multiPermissionsApp,
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

    private const bool DebugDetails = true;

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
        multiPermissionsApp.ThrowIfNotAllowedInApp(block.Context, GrantSets.WritePublished);
        return blockEditorSelectorLazy.Value
            .GetEditor(block)
            .Publish(part, index);
    }

    public AjaxRenderDto RenderForAjax(int templateId, string lang, string root, string edition)
    {
        var l = Log.Fn<AjaxRenderDto>($"{nameof(templateId)}: {templateId}; {nameof(lang)}: {lang}; {nameof(root)}: {root}; {nameof(edition)}: {edition}");
        l.A("1. Get Render result");
        var result = RenderToResult(templateId, lang, edition);

        l.A("2.1. Build Resources");
        var resources = new List<AjaxResourceDto>();
        var ver = EavSystemInfo.VersionWithStartUpBuild;
        if (result.Features?.Contains(SxcPageFeatures.TurnOn) == true)
            resources.Add(new()
            {
                Url = UrlHelpers.QuickAddUrlParameter(root.SuffixSlash() + SxcPageFeatures.TurnOn.UrlInDist, "v", ver)
            });

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
        var mergedFeatures  = string.Join("\n", result.FeaturesFromResources?.Select(mc => mc.Html) ?? []);

        l.A("4.1. Process optimizers");
        var renderResult = optimizerLazy.Value.Process(mergedFeatures, new(extractAll: true));
        var rest = renderResult.Html;
        if (!string.IsNullOrWhiteSpace(rest)) 
            l.A("Warning: Rest after extraction should be empty - not handled ATM");

        l.A("4.2. Add more resources based on processed");
        resources.AddRange(renderResult.Assets
            .Select(asset => new AjaxResourceDto
            {
                Url = asset.Url,
                Type = asset.IsJs ? "js" : "css",
                Attributes = asset.HtmlAttributes,
            })
        );

        if (DebugDetails)
        {
            l.A("5. Debug details");
            foreach (var ajaxResourceDto in resources)
            {
                l.A($"Url: {ajaxResourceDto.Url}");
            }
        }

        // Undo Dnn CDF ~/ prefix which is added in the process in the background while rendering,
        // to fix a CDF issue where paths with a space need "~/" to work and not be replaced with %20 
        // and contain spaces: https://github.com/2sic/2sxc/issues/1566
        // This is hacky, since we're first patching it and here unpatching it again
        // but it's really difficult to get the other place to know that we're in the ajax call.
        resources = resources
            .Select(res => res with { Url = res.Url != null ? res.Url.TrimStart('~') : res.Url })
            .ToList();

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