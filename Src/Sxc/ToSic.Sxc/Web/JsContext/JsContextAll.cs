using Newtonsoft.Json;
using ToSic.Eav.Data.Shared;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit.ClientContextInfo;

namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextAll : HasLog
    {
        private readonly JsContextLanguage _jsLangCtx;
        public JsContextEnvironment Environment;
        public JsContextUser User;
        public JsContextLanguage Language;
        
        [JsonProperty("contentBlockReference")]
        public ContentBlockReferenceDto ContentBlockReference; // todo: still not sure if these should be separate...
        
        [JsonProperty("contentBlock")]
        public ContentBlockDto ContentBlock;
        // ReSharper disable once InconsistentNaming
        public ErrorDto error;

        public UiDto Ui;

        public JsContextAll(JsContextLanguage jsLangCtx) : base("Sxc.CliInf")
        {
            _jsLangCtx = jsLangCtx;
        }

        public JsContextAll Init(string systemRootUrl, IBlock block, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var ctx = block.Context;

            Environment = new JsContextEnvironment(systemRootUrl, ctx);
            Language = _jsLangCtx.Init(ctx.Site, block.ZoneId);

            // New in v13 - if the view is from remote, don't allow design
            var view = block.View; // can be null
            var canDesign = !view?.Entity.HasAncestor();

            User = new JsContextUser(ctx.User, canDesign);

            ContentBlockReference = new ContentBlockReferenceDto(block, ctx.Publishing.Mode);
            ContentBlock = new ContentBlockDto(block);
            Ui = new UiDto(((BlockBuilder)block.BlockBuilder)?.UiAutoToolbar ?? false);

            error = new ErrorDto(block);
            return this;
        }
    }
}
