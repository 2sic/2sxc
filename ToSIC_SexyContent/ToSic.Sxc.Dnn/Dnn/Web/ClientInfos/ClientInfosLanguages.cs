using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Run;


namespace ToSic.Sxc.Dnn.Web.ClientInfos
{
    public class ClientInfosLanguages
    {
        public string Current;
        public string Primary;
        public IEnumerable<ClientInfoLanguage> All;

        public ClientInfosLanguages(ITenant tenant, int zoneId)
        {
            // Don't use PortalSettings, as that provides a wrong ps.CultureCode.ToLower();
            Current = System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToLower();
            Primary = tenant.DefaultLanguage;
            All = Eav.Factory.Resolve<IZoneMapper>()
                .CulturesWithState(tenant.Id, zoneId)
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
