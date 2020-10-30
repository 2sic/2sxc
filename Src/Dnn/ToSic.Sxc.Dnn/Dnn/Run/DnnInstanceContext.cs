using System.Collections.Generic;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnContext: InstanceContext
    {
        public DnnContext(ISite site, IContainer container, IUser user, List<KeyValuePair<string, string>> overrideParams = null) 
            : base(site, null, container, user, null)
        {
            var activeTab = (site as Site<PortalSettings>)?.UnwrappedContents?.ActiveTab;
            // the FullUrl will throw an error in search scenarios
            string fullUrl = null;
            try { fullUrl = activeTab?.FullUrl; } catch {  /* ignore */ }
            var page = new DnnPage(activeTab?.TabID ?? Eav.Constants.NullId, fullUrl);
            if (overrideParams != null)
                page.Parameters = overrideParams;
            Page = page;
        }
    }
}
