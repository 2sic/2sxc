using ToSic.Sxc.Mvc.Dev;
using ToSic.Sxc.Mvc.Web;

namespace ToSic.Sxc.Mvc.TestStuff
{
    public class MvcPortalSettings
    {
        public MvcPortalSettings(int zoneId = TestIds.PrimaryZone)
        {
            Id = zoneId;
        }

        public string DefaultLanguage => TestIds.DefaultLanguage;
        public int Id { get; }

        public string Name => "Fake MVC Tenant Name";

        public string HomePath => MvcConstants.WwwRoot;
    }
}
