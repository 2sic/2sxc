using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Eav.Security.Internal;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Backend.InPage;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Cms.Internal.Publishing;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.PageFeatures;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Backend.ContentBlocks;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ContentBlockBackend : BlockWebApiBackendBase
{
    private readonly GenWorkPlus<WorkViews> _workViews;
    private readonly GenWorkDb<WorkBlocksMod> _workBlocksMod;
    private readonly LazySvc<BlockEditorSelector> _blockEditorSelectorLazy;
    private readonly Generator<BlockFromEntity> _entityBlockGenerator;

    #region constructor / DI

    public ContentBlockBackend(
        GenWorkPlus<WorkViews> workViews,
        Generator<MultiPermissionsApp> multiPermissionsApp, 
        IPagePublishing publishing, 
        GenWorkDb<WorkBlocksMod> workBlocksMod,
        ISxcContextResolver ctxResolver, 
        LazySvc<IBlockResourceExtractor> optimizerLazy,
        LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
        AppWorkContextService appWorkCtxService,
        Generator<BlockFromEntity> entityBlockGenerator)
        : base(multiPermissionsApp, appWorkCtxService, ctxResolver, "Bck.FldLst")
    {
        ConnectServices(
            _optimizer = optimizerLazy,
            _workBlocksMod = workBlocksMod,
            _workViews = workViews,
            _publishing = publishing,
            _entityBlockGenerator = entityBlockGenerator,
            _blockEditorSelectorLazy = blockEditorSelectorLazy
        );
    }

    private readonly LazySvc<IBlockResourceExtractor> _optimizer;
    private readonly IPagePublishing _publishing;


    #endregion


    public IRenderResult NewBlockAndRender(int parentId, string field, int index, string app = "", Guid? guid = null) 
    {
        var entityId = NewBlock(parentId, field, index, app, guid);

        // now return a rendered instance
        var newContentBlock = _entityBlockGenerator.New().Init(Block, null, entityId);
        return newContentBlock.BlockBuilder.Run(true, specs: new());
    }

    // todo: probably move to CmsManager.Block
    public int NewBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null) 
        => _workBlocksMod.New(AppWorkCtxDb).NewBlockReference(parentId, field, sortOrder, app, guid);

    public void AddItem(int? index = null)
    {
        Log.A($"add order:{index}");
        // use dnn versioning - this is always part of page
        _publishing.DoInsidePublishing(ContextOfBlock, _ 
            => _workBlocksMod.New(AppWorkCtxDb).AddEmptyItem(Block.Configuration, index, Block.Context.Publishing.ForceDraft));
    }

        
    public bool PublishPart(string part, int index)
    {
        Log.A($"try to publish #{index} on '{part}'");
        ThrowIfNotAllowedInApp(GrantSets.WritePublished);
        return _blockEditorSelectorLazy.Value.GetEditor(Block).Publish(part, index);
    }

    public AjaxRenderDto RenderForAjax(int templateId, string lang, string root, string edition)
    {
        var l = Log.Fn<AjaxRenderDto>();
        l.A("1. Get Render result");
        var result = RenderToResult(templateId, lang, edition);

        l.A("2.1. Build Resources");
        var resources = new List<AjaxResourceDtoWIP>();
        var ver = EavSystemInfo.VersionWithStartUpBuild;
        if (result.Features.Contains(SxcPageFeatures.TurnOn))
            resources.Add(new() { Url = UrlHelpers.QuickAddUrlParameter(root.SuffixSlash() + SxcPageFeatures.TurnOn.UrlWip, "v", ver) });

        l.A("2.2. Add JS & CSS which were stripped before");
        resources.AddRange(result.Assets.Select(asset => new AjaxResourceDtoWIP
        {
            // Note: Url can be empty if it has contents
            Url = string.IsNullOrWhiteSpace(asset.Url) ? null : UrlHelpers.QuickAddUrlParameter(asset.Url, "v", ver), 
            Type = asset.IsJs ? "js" : "css",
            Contents = asset.Content,
            Attributes = asset.HtmlAttributes,
        }));

        l.A("3. Add manual resources (fancybox etc.)");
        // First get all the parts out of HTML, as the configuration is still stored as plain HTML
        var mergedFeatures  = string.Join("\n", result.FeaturesFromSettings.Select(mc => mc.Html));

        l.A("4.1. Process optimizers");
        var renderResult = _optimizer.Value.Process(mergedFeatures, new(extractAll: true));
        var rest = renderResult.Html;
        if (!string.IsNullOrWhiteSpace(rest)) 
            l.A("Warning: Rest after extraction should be empty - not handled ATM");

        l.A("4.2. Add more resources based on processed");
        resources.AddRange(renderResult.Assets.Select(asset => new AjaxResourceDtoWIP
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
        var callLog = Log.Fn<IRenderResult>($"{nameof(templateId)}:{templateId}, {nameof(lang)}:{lang}");

        // if a preview templateId was specified, swap to that
        if (templateId > 0)
        {
            var template = _workViews.New(AppWorkCtxPlus).Get(templateId);
            template.Edition = edition;
            Block.View = template;
        }

        var result = Block.BlockBuilder.Run(true, specs: new());
        return callLog.ReturnAsOk(result);
    }

}