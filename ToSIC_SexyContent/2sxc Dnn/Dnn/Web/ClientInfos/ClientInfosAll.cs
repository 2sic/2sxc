using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
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

        public ClientInfosAll(string systemRootUrl, PortalSettings ps, IContainer mic, Blocks.IBlockBuilder blockBuilder, UserInfo user, int zoneId, bool isCreated, ILog parentLog)
            : base("Sxc.CliInf", parentLog, "building entire client-context")
        {
            var versioning = blockBuilder.Environment.PagePublishing;

            Environment = new ClientInfosEnvironment(systemRootUrl, ps, mic, blockBuilder);
            Language = new ClientInfosLanguages(ps, zoneId);
            User = new ClientInfosUser(user);

            ContentBlock = new ClientInfoContentBlock(blockBuilder.Block, null, 0, versioning.Requirements(mic.Id));
            ContentGroup = new ClientInfoContentGroup(blockBuilder, isCreated);
            Ui = new Ui(((Blocks.BlockBuilder)blockBuilder).UiAutoToolbar);

            error = new ClientInfosError(blockBuilder.Block);
        }
    }
}
