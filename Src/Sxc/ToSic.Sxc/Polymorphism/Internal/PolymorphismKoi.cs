using Connect.Koi;
using static System.StringComparison;

namespace ToSic.Sxc.Polymorphism.Internal;

[PolymorphResolver("Koi")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PolymorphismKoi(ICss pageCss) : IPolymorphismResolver
{
    public string NameId => "Koi";

    public const string ModeCssFramework= "cssFramework";

    public string Edition(string parameters, ILog log)
    {
        var l = log.Fn<string>();
        if (!string.Equals(parameters, ModeCssFramework, InvariantCultureIgnoreCase))
            return l.ReturnNull("unknown param");
        // Note: this is still using the global object which we want to get rid of
        // But to use DI, we must refactor Polymorphism
        var cssFramework = pageCss.Framework; // Connect.Koi.Koi.Css;
        return l.Return(cssFramework, cssFramework);
    }

    public bool IsViable() => true;

    public int Priority => 10;
}