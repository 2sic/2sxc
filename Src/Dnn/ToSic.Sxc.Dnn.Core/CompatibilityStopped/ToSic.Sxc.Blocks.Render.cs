﻿

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Blocks;

/// <summary>
/// Deprecated since v12, announced for removal in v15, removed in v20.
/// </summary>
[PrivateApi]
[Obsolete]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class Render
{
    internal static string GenerateMessage(string fullNameSpace)
        => $"The old {fullNameSpace} API has been deprecated since v12 and announced for removal in v15. They were removed in v20. " +
           $"Please use the @Kit.Render.One(...)/All(...) instead. " +
           $"For guidance, see https://go.2sxc.org/brc-20-static-render";

    [Obsolete]
    public static string One(object parent, object noParamOrder = default, object item = null, string field = null, object newGuid = null)
        => throw new(GenerateMessage("ToSic.Sxc.Blocks.One()"));

    [Obsolete]
    public static string All(object parent, object noParamOrder = default, string field = null, string apps = null, int max = 100, string merge = null)
        => throw new(GenerateMessage("ToSic.Sxc.Blocks.All()"));

}