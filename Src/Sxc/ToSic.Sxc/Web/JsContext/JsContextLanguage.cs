using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextLanguage
    {
        public string Current;
        public string Primary;
        public IEnumerable<ClientInfoLanguage> All;

        public JsContextLanguage(IServiceProvider serviceProvider, ISite site, int zoneId)
        {
            // Don't use PortalSettings, as that provides a wrong ps.CultureCode.ToLower();
            Current = System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToLower();
            Primary = site.DefaultLanguage;
            All = serviceProvider.Build<IZoneMapper>()
                .CulturesWithState(site.Id, zoneId)
                .Where(c => c.Active)
                .Select(c => new ClientInfoLanguage { key = c.Key.ToLower(), name = c.Text });
        }
    }

    public class ClientInfoLanguage
    {
        // key and name must be lowercase, has side effects in EAV
        // ReSharper disable InconsistentNaming
        public string key;
        public string name;
        // ReSharper restore InconsistentNaming
    }
}
