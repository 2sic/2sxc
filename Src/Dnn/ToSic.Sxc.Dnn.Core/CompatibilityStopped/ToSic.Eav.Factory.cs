// ReSharper disable once CheckNamespace
namespace ToSic.Eav;

/// <summary>
/// The Eav DI Factory, used to construct various objects through [Dependency Injection](xref:NetCode.DependencyInjection.Index).
///
/// If possible avoid using this, as it's a workaround for code which is outside the normal Dependency Injection and therefor a bad pattern.
/// </summary>
[PrivateApi("Up to v19 used to PublicApi(Careful - obsolete!)")]
[Obsolete]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class Factory
{
    internal static string GenerateMessage(string fullNameSpace, string link)
        => $"The old '{fullNameSpace}' API has been deprecated since v13 and announced for removal in v15. They were removed in v20. " +
           $"For guidance, see {link}.";

    /// <summary>
    /// Dependency Injection resolver with a known type as a parameter.
    /// </summary>
    /// <typeparam name="T">The type / interface we need.</typeparam>
    [Obsolete]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static T Resolve<T>()
        => throw new NotSupportedException(GenerateMessage("ToSic.Eav.Factory.Resolve()", "https://go.2sxc.org/brc-13-eav-factory"));

}