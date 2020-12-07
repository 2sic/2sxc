using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Edit.ClientContextInfo;

namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextAll : HasLog
    {
        private readonly JsContextLanguage _jsLangCtx;
        public JsContextEnvironment Environment;
        public JsContextUser User;
        public JsContextLanguage Language;
        public ClientInfoContentBlock ContentBlock; // todo: still not sure if these should be separate...
        public ClientInfoContentGroup ContentGroup;
        // ReSharper disable once InconsistentNaming
        public ClientInfosError error;

        public Ui Ui;

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
            User = new JsContextUser(ctx.User);

            ContentBlock = new ClientInfoContentBlock(block, null, 0, ctx.Publishing.Mode);
            ContentGroup = new ClientInfoContentGroup(block);
            Ui = new Ui(((BlockBuilder)block.BlockBuilder)?.UiAutoToolbar ?? false);

            error = new ClientInfosError(block);
            return this;
        }
    }
}
