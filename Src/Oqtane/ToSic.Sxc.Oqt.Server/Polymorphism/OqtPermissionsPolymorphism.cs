using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Sxc.Polymorphism;
using static System.StringComparison;

namespace ToSic.Sxc.Oqt.Server.Polymorphism;

[PolymorphResolver("Permissions")]
public class Permissions : IResolver
{
    private readonly IUser _oqtUser;

    public Permissions(IUser oqtUser)
    {
        _oqtUser = oqtUser;
    }

    public string Name => "Permissions";

    public const string ModeIsSuperUser = "IsSuperUser";

    public string Edition(string parameters, ILog log)
    {
        var wrapLog = log.Fn<string>();
        if (!string.Equals(parameters, ModeIsSuperUser, InvariantCultureIgnoreCase))
            return wrapLog.ReturnNull("unknown param");
        var isSuper = _oqtUser.IsSystemAdmin;
        var result = isSuper ? "staging" : "live";
        return wrapLog.ReturnAndLog(result);
    }
}