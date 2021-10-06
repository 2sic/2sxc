using System.Collections.Generic;
using System.Linq;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web.Parameters;


namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextEnvironment
    {
        public int WebsiteId;       // aka PortalId
        public string WebsiteUrl;
        public int PageId;          // aka TabId
        public string PageUrl;
        // ReSharper disable once InconsistentNaming
        public IEnumerable<KeyValuePair<string, string>> parameters;

        public int InstanceId;      // aka ModuleId

        public string SxcVersion;

        public string SxcRootUrl;

        public bool IsEditable;

        public JsContextEnvironment(string systemRootUrl, IContextOfBlock ctx)
        {
            WebsiteId = ctx.Site.Id;

            WebsiteUrl = "//" + ctx.Site.UrlRoot + "/";

            PageId = ctx.Page.Id;
            PageUrl = ctx.Page.Url;

            InstanceId = ctx.Module.Id;

            SxcVersion = Settings.Version.ToString();

            SxcRootUrl = systemRootUrl;

            var userMayEdit = ctx.UserMayEdit;

            IsEditable = userMayEdit;
            parameters = ctx.Page.ParametersInternalOld?.Where(p => p.Key != OriginalParameters.NameInUrlForOriginalParameters);
        }
    }

}
