using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Sxc.Polymorphism;
using static System.StringComparison;

namespace ToSic.Sxc.Oqt.Server.Polymorphism;

[PolymorphResolver("Permissions")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class Permissions(IUser oqtUser) : IResolver
{
    public string Name => "Permissions";

    public const string ModeIsSuperUser = "IsSuperUser";

    public string Edition(string parameters, ILog log)
    {
        var wrapLog = log.Fn<string>();
        if (!string.Equals(parameters, ModeIsSuperUser, InvariantCultureIgnoreCase))
            return wrapLog.ReturnNull("unknown param");
        var isSuper = oqtUser.IsSystemAdmin;
        var result = isSuper ? "staging" : "live";
        return wrapLog.ReturnAndLog(result);
    }
}