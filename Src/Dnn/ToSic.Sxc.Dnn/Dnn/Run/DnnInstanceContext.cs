using System.Collections.Generic;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnContext: InstanceContext
    {
        public DnnContext(ITenant tenant, IContainer container, IUser user, List<KeyValuePair<string, string>> overrideParams = null) 
            : base(tenant, null, container, user)
        {
            var activeTab = (tenant as Tenant<PortalSettings>)?.UnwrappedContents?.ActiveTab;
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
