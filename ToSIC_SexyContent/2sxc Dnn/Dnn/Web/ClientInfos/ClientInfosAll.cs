using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.SexyContent.Edit.ClientContextInfo;
using ToSic.SexyContent.Environment.Dnn7;
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

        public ClientInfosAll(string systemRootUrl, PortalSettings ps, IContainer mic, Sxc.Blocks.IBlockBuilder cms, UserInfo uinfo, int zoneId, bool isCreated, bool autoToolbar, ILog parentLog)
            : base("Sxc.CliInf", parentLog, "building entire client-context")
        {
            var versioning = cms.Environment.PagePublishing;

            Environment = new ClientInfosEnvironment(systemRootUrl, ps, mic, cms);
            Language = new ClientInfosLanguages(ps, zoneId);
            User = new ClientInfosUser(uinfo);

            ContentBlock = new ClientInfoContentBlock(cms.Block, null, 0, versioning.Requirements(mic.Id));
            ContentGroup = new ClientInfoContentGroup(cms, isCreated);
            Ui = new Ui(((Sxc.Blocks.BlockBuilder)cms).UiAutoToolbar);

            error = new ClientInfosError(cms.Block);
        }
    }
}
