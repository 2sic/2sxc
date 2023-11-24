using DotNetNuke.Entities.Portals;
using ToSic.Lib.Logging;
using static System.StringComparison;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Polymorphism
{
    [PolymorphResolver("Permissions")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class Permissions : IResolver
    {
        public string Name => "Permissions";

        public const string ModeIsSuperUser = "IsSuperUser";

        public string Edition(string parameters, ILog log)
        {
            var l = log.Fn<string>();
            if (!string.Equals(parameters, ModeIsSuperUser, InvariantCultureIgnoreCase))
                return l.ReturnNull("unknown param");
            var isSuper = PortalSettings.Current?.UserInfo?.IsSuperUser ?? false;
            var result = isSuper ? "staging" : "live";
            return l.Return(result, result);
        }
    }
}