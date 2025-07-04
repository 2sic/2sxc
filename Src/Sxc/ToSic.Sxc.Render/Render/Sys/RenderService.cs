﻿using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Data;
using ToSic.Sxc.Render.Sys.RenderBlock;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.Render.PageFeatures;

namespace ToSic.Sxc.Render.Sys;

/// <summary>
/// Block-Rendering system. It's responsible for taking a Block and delivering HTML for the output. <br/>
/// It's used for InnerContent, so that Razor-Code can easily render additional content blocks. <br/>
/// See also [](xref:Basics.Cms.InnerContent.Index)
/// </summary>
[PrivateApi("Hide Implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class RenderService(RenderService.MyServices services) : ServiceWithContext("Sxc.RndSvc", connect: [services]),
    IRenderService
// #RemoveBlocksIRenderService
//#if NETFRAMEWORK
//#pragma warning disable CS0618
//   , Blocks.IRenderService
//#pragma warning restore CS0618
//#endif
{
    #region Constructor & ConnectToRoot

    public class MyServices(
        Generator<IEditService> editGenerator,
        LazySvc<IModuleAndBlockBuilder> builder,
        Generator<SimpleRenderer> simpleRenderer,
        Generator<InTextContentBlockRenderer> inTextRenderer,
        Generator<IBlockBuilder> blockBuilderGenerator,
        LazySvc<ILogStore> logStore)
        : MyServicesBase(connect: [editGenerator, builder, simpleRenderer, inTextRenderer, logStore, blockBuilderGenerator])
    {
        public Generator<InTextContentBlockRenderer> InTextRenderer { get; } = inTextRenderer;
        public Generator<IBlockBuilder> BlockBuilderGenerator { get; } = blockBuilderGenerator;
        public Generator<SimpleRenderer> SimpleRenderer { get; } = simpleRenderer;
        public Generator<IEditService> EditGenerator { get; } = editGenerator;
        public LazySvc<IModuleAndBlockBuilder> Builder { get; } = builder;
        public LazySvc<ILogStore> LogStore { get; } = logStore;
    }

    // ReSharper disable once InconsistentNaming

    // #RemoveBlocksIRenderService
    //public override void ConnectToRoot(IExecutionContext exCtx)
    //{
    //    base.ConnectToRoot(exCtx);
    //    _logIsInHistory = true; // if we link it to a parent, we don't need to add own entry in log history
    //}


    #endregion

    // #RemoveBlocksIRenderService
    //#region Ensure Logging in Insight
    //private bool _logIsInHistory;

    //protected void MakeSureLogIsInHistory()
    //{
    //    if (_logIsInHistory) return;
    //    _logIsInHistory = true;
    //    services.LogStore.Value.Add("render-service", Log);
    //}

    //#endregion


    /// <summary>
    /// Render one content block
    /// This is accessed through DynamicEntity.Render()
    /// At the moment it MUST stay internal, as it's not clear what API we want to surface
    /// </summary>
    /// <param name="parent">The parent-item containing the content-blocks and providing edit-context</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="item">The content-block item to render. Optional, by default the same item is used as the context.</param>
    /// <param name="data">TODO V16.00</param>
    /// <param name="field">Optional: </param>
    /// <param name="newGuid">Internal: this is the guid given to the item when being created in this block. Important for the inner-content functionality to work. </param>
    /// <returns></returns>
    public IRawHtmlString One(
        ICanBeItem parent,
        NoParamOrder noParamOrder = default,
        ICanBeEntity? item = null,
        object? data = null,
        string? field = null,
        Guid? newGuid = null)
    {
        item ??= parent.Item;
        // #RemoveBlocksIRenderService
        //MakeSureLogIsInHistory();
        //var block = parent.GetRequiredBlockForRender();
        var block = ExCtx.GetState<IBlock>();
        var simpleRenderer = services.SimpleRenderer.New();
        return Tag.Custom(field == null
            ? simpleRenderer.Render(block, item.Entity, data: data) // without field edit-context
            : simpleRenderer.RenderWithEditContext(block, parent, item, field, newGuid, GetEditService(block), data)); // with field-edit-context data-list-context
    }

    /// <summary>
    /// Render content-blocks into a larger html-block containing placeholders
    /// </summary>
    /// <param name="parent">The parent-item containing the content-blocks and providing edit-context</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="field">Required: Field containing the content-blocks. </param>
    /// <param name="max">BETA / WIP</param>
    /// <param name="merge">Optional: html-text containing special placeholders.</param>
    /// <param name="apps">BETA / WIP</param>
    /// <returns></returns>
    /// <remarks>
    /// * Changed result object to `IRawHtmlString` in v16.02 from `IHybridHtmlString`
    /// </remarks>
    public IRawHtmlString All(
        ICanBeItem parent,
        NoParamOrder noParamOrder = default,
        string? field = null,
        string? apps = null,
        int max = 100,
        string? merge = null)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentNullException("To render all items, you must specify a field where the items are stored.", nameof(field));

        // #RemoveBlocksIRenderService
        //MakeSureLogIsInHistory();
        //var block = parent.GetRequiredBlockForRender();
        var block = ExCtx.GetState<IBlock>(); 
        var editService = GetEditService(block);
        return Tag.Custom(merge == null
            ? services.SimpleRenderer.New().RenderListWithContext(block, parent.Entity, field, apps, max, editService)
            : services.InTextRenderer.New().RenderMerge(block, parent.Entity, field, merge, editService)
        );
    }


    /// <inheritdoc />
    public virtual IRenderResult Module(int pageId, int moduleId, NoParamOrder noParamOrder = default, object? data = null)
    {
        var l = Log.Fn<IRenderResult>($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}");
        // #RemoveBlocksIRenderService
        //MakeSureLogIsInHistory();

        // This service is often used from a theme/skin, in which case it doesn't have a ExecutionContext,
        // which also means that it was not logged - which we're doing here.
        if (ExCtxOrNull == null)
            services.LogStore.Value.Add("render-service", Log);

        var moduleBlock = services.Builder.Value.BuildBlock(pageId, moduleId);

        moduleBlock.BlockFeatureKeys.Add(SxcPageFeatures.JsApiOnModule.NameId);
        var builder = services.BlockBuilderGenerator.New().Setup(moduleBlock);
        var result = builder.Run(true, specs: new() { Data = data });

        return l.ReturnAsOk(result);
    }

    /// <summary>
    /// create edit-object which is necessary for context attributes
    /// We need a new one for each parent
    /// </summary>
    private IEditService GetEditService(IBlock blockOrNull)
    {
        // If we have a dyn-code, use that
        return ExCtx.GetService<IEditService>(reuse: true);
        
        // #RemoveBlocksIRenderService
        //var editSvc = ExCtxOrNull?.GetService<IEditService>(reuse: true);
        //if (editSvc != null)
        //    return editSvc;

        //// Otherwise create a new one - even though it's not clear if this would have any real effect
        //var newEdit = ExCtxOrNull?.GetService<IEditService>() ?? services.EditGenerator.New();
        //newEdit = ((IEditServiceSetup)newEdit).SetBlock(ExCtxOrNull, blockOrNull);
        //return newEdit;
    }
}