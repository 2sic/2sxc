using Newtonsoft.Json;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.EditService
{
    public partial class EditService : HasLog, IEditService
    {
        public EditService(LazyInit<IRenderingHelper> renderHelper) : base("Sxc.Edit")
        {
            _renderHelper = renderHelper.SetInit(h => h.Init(Block, Log));
        }
        private readonly LazyInit<IRenderingHelper> _renderHelper;

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            Log.LinkTo(codeRoot.Log);
            SetBlock(codeRoot.Block);
        }

        public IEditService SetBlock(IBlock block)
        {
            Block = block;
            Enabled = Block?.Context.UserMayEdit ?? false;
            if(Log.Parent == null && block != null) Log.LinkTo(block.Log);
            return this;
        }

        protected IBlock Block;

        #region Attribute-helper

        /// <inheritdoc/>
        public IHybridHtmlString Attribute(string name, string value)
            => !Enabled ? null : Build.Attribute(name, value);

        /// <inheritdoc/>
        public IHybridHtmlString Attribute(string name, object value)
            => !Enabled ? null : Build.Attribute(name, JsonConvert.SerializeObject(value));

        #endregion Attribute Helper

    }
}
