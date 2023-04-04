using System;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
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
        internal const string InputTypeForContentBlocksField = "entity-content-blocks";

        #region Constructor & ConnectToRoot

        public class MyServices: MyServicesBase
        {
            public Generator<InTextContentBlockRenderer> InTextRenderer { get; }
            public Generator<SimpleRenderer> SimpleRenderer { get; }
            public Generator<IEditService> EditGenerator { get; }
            public LazySvc<IModuleAndBlockBuilder> Builder { get; }
            public LazySvc<ILogStore> LogStore { get; }

            public MyServices(Generator<IEditService> editGenerator,
                LazySvc<IModuleAndBlockBuilder> builder,
                Generator<SimpleRenderer> simpleRenderer,
                Generator<InTextContentBlockRenderer> inTextRenderer,
                LazySvc<ILogStore> logStore
            )
            {
                ConnectServices(
                    EditGenerator = editGenerator,
                    Builder = builder,
                    SimpleRenderer = simpleRenderer,
                    InTextRenderer = inTextRenderer,
                    LogStore = logStore
                );
            }
        }

        public RenderService(MyServices services) : base("Sxc.RndSvc")
        {
            _Deps = services.ConnectServices(Log);
        }

        private readonly MyServices _Deps;

        public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            base.ConnectToRoot(codeRoot);
            _logIsInHistory = true; // if we link it to a parent, we don't need to add own entry in log history
        }

        private bool _logIsInHistory;

        #endregion

        #region Ensure Logging in Insight

        protected void MakeSureLogIsInHistory()
        {
            if (_logIsInHistory) return;
            _logIsInHistory = true;
            _Deps.LogStore.Value.Add("render-service", Log);
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
            object data = null,
            string field = null,
            Guid? newGuid = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(One), $"{nameof(item)},{nameof(field)},{nameof(newGuid)}");
            item = item ?? parent;
            MakeSureLogIsInHistory();
            var simpleRenderer = _Deps.SimpleRenderer.New();
            return new HybridHtmlString(field == null
                ? simpleRenderer.Render(parent._Services.BlockOrNull, item.Entity, data: data) // without field edit-context
                : simpleRenderer.RenderWithEditContext(parent, item, field, newGuid, GetEdit(parent), data)); // with field-edit-context data-list-context
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
            Eav.Parameters.Protect(noParamOrder, $"{nameof(field)},{nameof(merge)}");
            if (string.IsNullOrWhiteSpace(field)) throw new ArgumentNullException(nameof(field));

            MakeSureLogIsInHistory();
            return new HybridHtmlString(merge == null
                    ? _Deps.SimpleRenderer.New().RenderListWithContext(parent, field, apps, max, GetEdit(parent))
                    : _Deps.InTextRenderer.New().RenderMerge(parent, field, merge, GetEdit(parent)));
        }


        /// <inheritdoc />
        public virtual IRenderResult Module(
            int pageId,
            int moduleId,
            string noParamOrder = Eav.Parameters.Protector,
            object data = null)
        {
            var l = Log.Fn<IRenderResult>($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}");
            Eav.Parameters.Protect(noParamOrder, $"{nameof(data)}");
            MakeSureLogIsInHistory();
            var block = _Deps.Builder.Value.GetProvider(pageId, moduleId).LoadBlock().BlockBuilder;
            var result = block.Run(true, data);
            return l.ReturnAsOk(result);
        }

        /// <summary>
        /// create edit-object which is necessary for context attributes
        /// We need a new one for each parent
        /// </summary>
        private IEditService GetEdit(DynamicEntity parent)
        {
            // If we have a dyn-code, use that
            if (_DynCodeRoot?.Edit != null) return _DynCodeRoot.Edit;

            // Otherwise create a new one - even though it's not clear if this would have any real effect
            var newEdit = _Deps.EditGenerator.New();
            newEdit.ConnectToRoot(_DynCodeRoot);
            return newEdit.SetBlock(_DynCodeRoot, parent._Services.BlockOrNull);
        }
    }
}
