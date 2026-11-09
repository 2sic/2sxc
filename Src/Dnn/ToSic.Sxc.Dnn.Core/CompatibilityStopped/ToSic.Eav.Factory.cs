#pragma warning disable IDE0130
namespace ToSic.Eav;
#pragma warning restore IDE0130

/// <summary>
/// Deprecated since v13, announced for removal in v15, removed in v20.
/// </summary>
[PrivateApi("Up to v19 used to PublicApi(Careful - obsolete!)")]
[Obsolete]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class Factory
{
    internal static string GenerateMessage(string fullNameSpace, string link)
        => $"The old '{fullNameSpace}' API has been deprecated since v13 and announced for removal in v15. They were removed in v20. " +
           $"For guidance, see {link}.";

    [Obsolete]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static T Resolve<T>()
        => throw new NotSupportedException(GenerateMessage("ToSic.Eav.Factory.Resolve()", "https://go.2sxc.org/brc-13-eav-factory"));

}