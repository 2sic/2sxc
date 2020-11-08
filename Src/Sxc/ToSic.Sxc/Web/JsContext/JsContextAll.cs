using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit.ClientContextInfo;

namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextAll : HasLog
    {
        public JsContextEnvironment Environment;
        public JsContextUser User;
        public JsContextLanguage Language;
        public ClientInfoContentBlock ContentBlock; // todo: still not sure if these should be separate...
        public ClientInfoContentGroup ContentGroup;
        // ReSharper disable once InconsistentNaming
        public ClientInfosError error;

        public Ui Ui;

        public JsContextAll(string systemRootUrl, IBlock block, ILog parentLog)
            : base("Sxc.CliInf", parentLog, "building entire client-context")
        {
            var ctx = block.Context;
            var versioning = block.Context.ServiceProvider.Build<IPagePublishingResolver>().Init(Log);

            Environment = new JsContextEnvironment(systemRootUrl, ctx, block);
            Language = new JsContextLanguage(ctx.ServiceProvider, ctx.Tenant, block.ZoneId);
            User = new JsContextUser(ctx.User);

            ContentBlock = new ClientInfoContentBlock(block, null, 0, versioning.Requirements(ctx.Container.Id));
            ContentGroup = new ClientInfoContentGroup(block);
            Ui = new Ui(((BlockBuilder)block.BlockBuilder)?.UiAutoToolbar ?? false);

            error = new ClientInfosError(block);
        }
    }
}
