using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Sxc.Blocks;

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

        public ClientInfosEnvironment(string systemRootUrl, IInstanceContext ctx, IBlockBuilder blockBuilder)
        {
            WebsiteId = ctx.Tenant.Id;

            WebsiteUrl = "//" + ctx.Tenant.Url + "/";

            PageId = ctx.Page.Id;
            PageUrl = ctx.Page.Url;

            InstanceId = ctx.Container.Id;

            SxcVersion = Settings.Version.ToString();

            SxcRootUrl = systemRootUrl;

            var userMayEdit = blockBuilder?.UserMayEdit ?? false;

            IsEditable = userMayEdit;
            parameters = blockBuilder?.Context.Page.Parameters;
        }
    }

}
