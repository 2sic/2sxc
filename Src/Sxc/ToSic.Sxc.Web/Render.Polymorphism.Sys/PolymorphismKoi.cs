using Connect.Koi;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Render.Polymorphism.Sys;

/// <summary>
/// Polymorphism resolver for CSS frameworks with Koi.
/// </summary>
/// <param name="pageCss"></param>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public class PolymorphismKoi(ICss pageCss) : IPolymorphismResolver
{
    public static string ResolverNameId = "koi";

    private const string ModeCssFramework= "cssFramework";

    public string? Edition(PolymorphismConfigurationModel config, string? overrule, ILog log)
    {
        var l = log.Fn<string?>();
        return !config.Parameters.EqualsInsensitive(ModeCssFramework)
            ? l.Return(overrule, "unknown param")
            : l.ReturnAndLog(overrule.NullIfNoValue() ?? pageCss.Framework);
    }
}