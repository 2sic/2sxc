using DotNetNuke.Entities.Portals;
using ToSic.Eav.Logging;
using static System.StringComparison;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Polymorphism
{
    [PolymorphResolver("Permissions")]
    public class Permissions : IResolver
    {
        public string Name => "Permissions";

        public const string ModeIsSuperUser = "IsSuperUser";

        public string Edition(string parameters, ILog log)
        {
            var wrapLog = log.Call<string>();
            if (!string.Equals(parameters, ModeIsSuperUser, InvariantCultureIgnoreCase))
                return wrapLog("unknown param", null);
            var isSuper = PortalSettings.Current?.UserInfo?.IsSuperUser ?? false;
            var result = isSuper ? "staging" : "live";
            return wrapLog(result, result);
        }
    }
}