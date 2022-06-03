using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web.Parameters;


namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextEnvironment
    {
        public int WebsiteId { get; }       // aka PortalId
        public string WebsiteUrl { get; }
        public int PageId { get; }          // aka TabId
        public string PageUrl { get; }
        // ReSharper disable once InconsistentNaming
        public IEnumerable<KeyValuePair<string, string>> parameters { get; }

        public int InstanceId { get; }      // aka ModuleId

        public string SxcVersion { get; }

        public string SxcRootUrl { get; }

        public bool IsEditable { get; }

        public JsContextEnvironment(string systemRootUrl, IContextOfBlock ctx)
        {
            WebsiteId = ctx.Site.Id;
            WebsiteUrl = "//" + ctx.Site.UrlRoot + "/";
            PageId = ctx.Page.Id;
            PageUrl = ctx.Page.Url;
            InstanceId = ctx.Module.Id;
            SxcVersion = EavSystemInfo.VersionWithStartUpBuild;
            SxcRootUrl = systemRootUrl;
            IsEditable = ctx.UserMayEdit;
            parameters = ctx.Page.Parameters?.Where(p => p.Key != OriginalParameters.NameInUrlForOriginalParameters);
        }
    }

}
