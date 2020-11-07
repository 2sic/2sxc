using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnContext
    {

        public static InstanceContext Create(ISite site, IContainer container, IUser user, IServiceProvider serviceProvider, List<KeyValuePair<string, string>> overrideParams = null) 
        {
            // Collect / assemble page information
            var activeTab = (site as Site<PortalSettings>)?.UnwrappedContents?.ActiveTab;
            // the FullUrl will throw an error in search scenarios
            string fullUrl = null;
            try { fullUrl = activeTab?.FullUrl; } catch {  /* ignore */ }
            overrideParams = overrideParams ?? serviceProvider.Build<IHttp>()?.QueryStringKeyValuePairs() ?? new List<KeyValuePair<string, string>>();
            var page = new SxcPage(activeTab?.TabID ?? Eav.Constants.NullId, fullUrl, overrideParams);

            return new InstanceContext(site, page, container, user, serviceProvider);
        }

    }
}
