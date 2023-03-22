using DotNetNuke.Entities.Portals;
using ToSic.Lib.Logging;
using static System.StringComparison;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Polymorphism
{
    [PolymorphResolver("Permissions")]
    public class Permissions : IResolver
    {
        public string Name => "Permissions";

        public const string ModeIsSuperUser = "IsSuperUser";

        public string Edition(string parameters, ILog log) => log.Func(() =>
        {
            if (!string.Equals(parameters, ModeIsSuperUser, InvariantCultureIgnoreCase))
                return (null, "unknown param");
            var isSuper = PortalSettings.Current?.UserInfo?.IsSuperUser ?? false;
            var result = isSuper ? "staging" : "live";
            return (result, result);
        });
    }
}