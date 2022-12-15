using System;
using ToSic.Eav.DI;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks.Renderers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// Block-Rendering system. It's responsible for taking a Block and delivering HTML for the output. <br/>
    /// It's used for InnerContent, so that Razor-Code can easily render additional content blocks. <br/>
    /// See also [](xref:Basics.Cms.InnerContent.Index)
    /// </summary>
    [PrivateApi("Hide Implementation")]
    public class RenderService: ServiceForDynamicCode,
        ToSic.Sxc.Services.IRenderService,
#pragma warning disable CS0618
        ToSic.Sxc.Blocks.IRenderService
#pragma warning restore CS0618
    {

        #region Constructor & ConnectToRoot

        public RenderService(
            GeneratorLog<IEditService> editGenerator,
            LazyInitLog<IModuleAndBlockBuilder> builder,
            GeneratorLog<BlockFromEntity> blkFrmEntGen,
            Lazy<History> historyLazy
        ) : base("Sxc.RndSvc")
            => ConnectServices(_historyLazy = historyLazy,
                _blkFrmEntGen = blkFrmEntGen,
                _builder = builder,
                _editGenerator = editGenerator
            );

        private readonly GeneratorLog<BlockFromEntity> _blkFrmEntGen;
        private readonly GeneratorLog<IEditService> _editGenerator;
        private readonly LazyInitLog<IModuleAndBlockBuilder> _builder;
        private readonly Lazy<History> _historyLazy;

        public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            base.ConnectToRoot(codeRoot);
            _logInitDone = true; // if we link it to a parent, we don't need to add own entry in log history
        }

        private bool _logInitDone;

        #endregion

        #region Ensure Logging in Insight

        protected void MakeSureLogIsInHistory()
        {
            if (_logInitDone) return;
            _logInitDone = true;
            _historyLazy.Value.Add("render-service", base.Log);
        }

        #endregion


        /// <summary>
        /// Render one content block
        /// This is accessed through DynamicEntity.Render()
        /// At the moment it MUST stay internal, as it's not clear what API we want to surface
        /// </summary>
        /// <param name="parent">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="item">The content-block item to render. Optional, by default the same item is used as the context.</param>
        /// <param name="field">Optional: </param>
        /// <param name="newGuid">Internal: this is the guid given to the item when being created in this block. Important for the inner-content functionality to work. </param>
        /// <returns></returns>
        public IHybridHtmlString One(DynamicEntity parent,
            string noParamOrder = Eav.Parameters.Protector,
            IDynamicEntity item = null,
            string field = null,
            Guid? newGuid = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(One), $"{nameof(item)},{nameof(field)},{nameof(newGuid)}");
            item = item ?? parent;
            MakeSureLogIsInHistory();
            return new HybridHtmlString(field == null
                ? Simple.Render(parent._Dependencies.BlockOrNull, item.Entity, _blkFrmEntGen) // without field edit-context
                : Simple.RenderWithEditContext(parent, item, field, newGuid, GetEdit(parent), _blkFrmEntGen)); // with field-edit-context data-list-context
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
        public IHybridHtmlString All(DynamicEntity parent,
            string noParamOrder = Eav.Parameters.Protector,
            string field = null,
            string apps = null,
            int max = 100,
            string merge = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(All), $"{nameof(field)},{nameof(merge)}");
            if (string.IsNullOrWhiteSpace(field)) throw new ArgumentNullException(nameof(field));

            MakeSureLogIsInHistory();
            return new HybridHtmlString(merge == null
                    ? Simple.RenderListWithContext(parent, field, apps, max, GetEdit(parent), _blkFrmEntGen)
                    : InTextContentBlocks.Render(parent, field, merge, GetEdit(parent), _blkFrmEntGen));
        }


        /// <inheritdoc />
        public virtual IRenderResult Module(int pageId, int moduleId)
        {
            MakeSureLogIsInHistory();
            var wrapLog = Log.Fn<IRenderResult>($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}");
            var block = _builder.Value.GetBlock(pageId, moduleId).BlockBuilder;
            var result = block.Run(true);
            return wrapLog.ReturnAsOk(result);
        }

        /// <summary>
        /// create edit-object which is necessary for context attributes
        /// We need a new one for each parent
        /// </summary>
        private IEditService GetEdit(DynamicEntity parent)
        {
            var newEdit = _editGenerator.New();
            newEdit.ConnectToRoot(_DynCodeRoot);
            return newEdit.SetBlock(_DynCodeRoot, parent._Dependencies.BlockOrNull);
        }
    }
}
