using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Edit.ClientContextInfo;
using ToSic.Sxc.Edit.ClientContextInfo;

namespace ToSic.SexyContent.Environment.Dnn7
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

        public ClientInfosAll(string systemRootUrl, PortalSettings ps, IInstanceInfo mic, SxcInstance sxc, UserInfo uinfo, int zoneId, bool isCreated, bool autoToolbar, Log parentLog)
            : base("Sxc.CliInf", parentLog, "building entire client-context")
        {
            var versioning = sxc.Environment.PagePublishing;

            Environment = new ClientInfosEnvironment(systemRootUrl, ps, mic, sxc);
            Language = new ClientInfosLanguages(ps, zoneId);
            User = new ClientInfosUser(uinfo);

            ContentBlock = new ClientInfoContentBlock(sxc.ContentBlock, null, 0, versioning.Requirements(mic.Id));
            ContentGroup = new ClientInfoContentGroup(sxc, isCreated);
            Ui = new Ui(true);

            error = new ClientInfosError(sxc.ContentBlock);
        }
    }
}
