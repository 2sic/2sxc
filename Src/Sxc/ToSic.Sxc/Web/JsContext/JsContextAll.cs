using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
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

        public JsContextAll(string systemRootUrl, IInstanceContext ctx, IBlockBuilder blockBuilder, ILog parentLog)
            : base("Sxc.CliInf", parentLog, "building entire client-context")
        {
            var versioning = Factory.Resolve<IPagePublishing>().Init(Log);

            Environment = new JsContextEnvironment(systemRootUrl, ctx, blockBuilder);
            Language = new JsContextLanguage(ctx.Tenant, blockBuilder.Block.ZoneId);
            User = new JsContextUser(ctx.User);

            ContentBlock = new ClientInfoContentBlock(blockBuilder.Block, null, 0, versioning.Requirements(ctx.Container.Id));
            ContentGroup = new ClientInfoContentGroup(blockBuilder);
            Ui = new Ui(((BlockBuilder)blockBuilder).UiAutoToolbar);

            error = new ClientInfosError(blockBuilder.Block);
        }
    }
}
