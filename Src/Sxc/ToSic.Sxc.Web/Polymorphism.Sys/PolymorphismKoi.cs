﻿using Connect.Koi;
using ToSic.Sys.Utils;
using static System.StringComparison;

namespace ToSic.Sxc.Polymorphism.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class PolymorphismKoi(ICss pageCss) : IPolymorphismResolver
{
    public string NameId => "Koi";

    public const string ModeCssFramework= "cssFramework";

    public string? Edition(PolymorphismConfiguration config, string? overrule, ILog log)
    {
        var l = log.Fn<string?>();
        if (!string.Equals(config.Parameters, ModeCssFramework, InvariantCultureIgnoreCase))
            return l.Return(overrule, "unknown param");
        // Note: this is still using the global object which we want to get rid of
        // But to use DI, we must refactor Polymorphism
        return l.ReturnAndLog(overrule.NullIfNoValue() ?? pageCss.Framework);
    }

    public bool IsViable() => true;

    public int Priority => 10;
}