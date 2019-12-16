using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using ToSic.SexyContent.Environment.Dnn7;


namespace ToSic.Sxc.Dnn.Web.ClientInfos
{
    public class ClientInfosLanguages
    {
        public string Current;
        public string Primary;
        public IEnumerable<ClientInfoLanguage> All;

        public ClientInfosLanguages(PortalSettings ps, int zoneId)
        {
            Current = System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToLower(); // 2016-05-09 had to ignore the Portalsettings, as that is wrong ps.CultureCode.ToLower();
            Primary = ps.DefaultLanguage.ToLower();
            All = new ZoneMapper().CulturesWithState(ps.PortalId, zoneId)
                .Where(c => c.Active)
                .Select(c => new ClientInfoLanguage { key = c.Key.ToLower(), name = c.Text });
        }
    }

    public class ClientInfoLanguage
    {
        // key and name must be lowercase, has side effects in EAV
        public string key;
        public string name;
    }
}
