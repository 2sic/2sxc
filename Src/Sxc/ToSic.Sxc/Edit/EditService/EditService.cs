using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.EditService
{
    public partial class EditService : ServiceForDynamicCode, IEditService
    {

        public EditService(IJsonService jsonService, LazySvc<IRenderingHelper> renderHelper) : base("Sxc.Edit")
        {
            _jsonService = jsonService;
            _renderHelper = renderHelper.SetInit(h => h.Init(Block, Log));
        }
        private readonly IJsonService _jsonService;
        private readonly LazySvc<IRenderingHelper> _renderHelper;

        public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            base.ConnectToRoot(codeRoot);
            SetBlock(codeRoot, codeRoot.Block);
        }

        public IEditService SetBlock(IDynamicCodeRoot codeRoot, IBlock block)
        {
            Block = block;
            var user = codeRoot?.CmsContext?.User;
            Enabled = Block?.Context.UserMayEdit ?? (user?.IsSiteAdmin ?? false) || (user?.IsSystemAdmin ?? false);
            if ((Log as Log)?.Parent == null && block != null)
                this.Init(block.Log);
            return this;
        }

        protected IBlock Block;

        #region Attribute-helper

        /// <inheritdoc/>
        public IHybridHtmlString Attribute(string name, string value)
            => !Enabled ? null : Build.Attribute(name, value);

        /// <inheritdoc/>
        public IHybridHtmlString Attribute(string name, object value)
            => !Enabled ? null : Build.Attribute(name, _jsonService.ToJson(value));

        #endregion Attribute Helper

    }
}
