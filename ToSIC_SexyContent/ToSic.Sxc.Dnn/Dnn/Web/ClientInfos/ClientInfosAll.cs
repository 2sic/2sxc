using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit.ClientContextInfo;

namespace ToSic.Sxc.Dnn.Web.ClientInfos
{
    public class ClientInfosAll : HasLog
    {
        public ClientInfosEnvironment Environment;
        public ClientInfosUser User;
        public ClientInfosLanguages Language;
        public ClientInfoContentBlock ContentBlock; // todo: still not sure if these should be separate...
        public ClientInfoContentGroup ContentGroup;
        // ReSharper disable once InconsistentNaming
        public ClientInfosError error;

        public Ui Ui;

        public ClientInfosAll(string systemRootUrl, IInstanceContext ctx, IBlockBuilder blockBuilder, int zoneId, bool isCreated, ILog parentLog)
            : base("Sxc.CliInf", parentLog, "building entire client-context")
        {
            var versioning = blockBuilder.Environment.PagePublishing;

            Environment = new ClientInfosEnvironment(systemRootUrl, ctx, blockBuilder);
            Language = new ClientInfosLanguages(ctx.Tenant, zoneId);
            User = new ClientInfosUser(ctx.User);

            ContentBlock = new ClientInfoContentBlock(blockBuilder.Block, null, 0, versioning.Requirements(ctx.Container.Id));
            ContentGroup = new ClientInfoContentGroup(blockBuilder, isCreated);
            Ui = new Ui(((BlockBuilder)blockBuilder).UiAutoToolbar);

            error = new ClientInfosError(blockBuilder.Block);
        }
    }
}
