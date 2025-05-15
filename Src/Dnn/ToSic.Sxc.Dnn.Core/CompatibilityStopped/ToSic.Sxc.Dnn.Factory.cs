using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn;

/// <summary>
/// Deprecated since v13, announced for removal in v15, removed in v20.
/// </summary>
[Obsolete]
[ShowApiWhenReleased(ShowApiMode.Never)]
public static class Factory
{
    internal static string GenerateMessage([CallerMemberName] string cName = default)
        => $"The old {nameof(Factory)}.{cName}() API has been deprecated since v13 and announced for removal in v15. They were removed in v20. " +
           $"Please use Dependency Injection and the IRenderService or IDynamicCodeService instead. " +
           $"For guidance, see https://go.2sxc.org/brc-13-dnn-factory";

    [Obsolete]
    public static object CmsBlock(int pageId, int modId)
        => throw new NotSupportedException(GenerateMessage());

    [Obsolete]
    public static object CmsBlock(int pageId, int modId, object parentLog)
        => throw new NotSupportedException(GenerateMessage());

    [Obsolete]
    public static object CmsBlock(object moduleInfo)
        => throw new NotSupportedException(GenerateMessage());

    [Obsolete]
    public static object CmsBlock(object module, object parentLog = null)
        => throw new NotSupportedException(GenerateMessage());

    [Obsolete]
    public static object DynamicCode(object blockBuilder)
        => throw new NotSupportedException(GenerateMessage());

    [Obsolete]
    public static object App(int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false, object parentLog = null)
        => throw new NotSupportedException(GenerateMessage());

    [Obsolete]
    public static object App(int zoneId, int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false, object parentLog = null)
        => throw new NotSupportedException(GenerateMessage());

    [Obsolete]
    public static object App(int appId, object ownerPortalSettings, bool unusedButKeepForApiStability = false, bool showDrafts = false, object parentLog = null)
        => throw new NotSupportedException(GenerateMessage());
}