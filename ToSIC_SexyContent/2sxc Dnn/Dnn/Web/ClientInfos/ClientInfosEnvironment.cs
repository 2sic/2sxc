using System.Collections.Generic;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Run;

// ReSharper disable InconsistentNaming
namespace ToSic.Sxc.Dnn.Web.ClientInfos
{
    public class ClientInfosEnvironment
    {
        public int WebsiteId;       // aka PortalId
        public string WebsiteUrl;
        public int PageId;          // aka TabId
        public string PageUrl;
        public IEnumerable<KeyValuePair<string, string>> parameters;

        public int InstanceId;      // aka ModuleId

        public string SxcVersion;

        public string SxcRootUrl;

        public bool IsEditable;

        public ClientInfosEnvironment(string systemRootUrl, PortalSettings ps, IContainer mic, Sxc.Blocks.ICmsBlock cmsInstance)
        {
            WebsiteId = ps.PortalId;

            WebsiteUrl = "//" + ps.PortalAlias.HTTPAlias + "/";

            PageId = mic.PageId;
            PageUrl = ps.ActiveTab.FullUrl;

            InstanceId = mic.Id;

            SxcVersion = Settings.Version.ToString();

            SxcRootUrl = systemRootUrl;

            var userMayEdit = cmsInstance?.UserMayEdit ?? false;

            IsEditable = userMayEdit;
            parameters = cmsInstance?.Parameters;
        }
    }

}
