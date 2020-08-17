using System.Collections.Generic;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
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
            var page = new DnnPage(activeTab?.TabID ?? AppConstants.AppIdNotFound, activeTab?.FullUrl);
            if (overrideParams != null)
                page.Parameters = overrideParams;
            Page = page;
        }
    }
}
