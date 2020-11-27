using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Sxc.Blocks;


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

        public JsContextEnvironment(string systemRootUrl, IContextOfBlock ctx, IBlock block)
        {
            WebsiteId = ctx.Site.Id;

            WebsiteUrl = "//" + ctx.Site.Url + "/";

            PageId = ctx.Page.Id;
            PageUrl = ctx.Page.Url;

            InstanceId = ctx.Module.Id;

            SxcVersion = Settings.Version.ToString();

            SxcRootUrl = systemRootUrl;

            var userMayEdit = block?.EditAllowed ?? false;

            IsEditable = userMayEdit;
            parameters = block?.Context.Page.Parameters;
        }
    }

}
