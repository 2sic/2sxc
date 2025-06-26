#if NETFRAMEWORK

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.ContentBlocks;

/// <summary>
/// Deprecated since v13, announced for removal in v15, removed in v20.
/// Remove this entire class (which currently just shows warnings) EOY 2025.
/// </summary>
[Obsolete]
[ShowApiWhenReleased(ShowApiMode.Never)]
public static class Render
{
    [Obsolete]
    public static string One(object context, object noParamOrder = default, object item = null, string field = null, Guid? newGuid = null)
        => throw new(ToSic.Sxc.Blocks.Render.GenerateMessage("ToSic.SexyContent.ContentBlocks.One()"));

    [Obsolete]
    public static string All(object context, object noParamOrder = default, string field = null, string merge = null)
        => throw new(ToSic.Sxc.Blocks.Render.GenerateMessage("ToSic.SexyContent.ContentBlocks.All()"));
}

#endif