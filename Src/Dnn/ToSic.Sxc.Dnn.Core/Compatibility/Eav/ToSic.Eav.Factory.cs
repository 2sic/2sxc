#if NETFRAMEWORK
using Microsoft.Extensions.DependencyInjection;
using ToSic.Lib.Code.Infos;
using ToSic.Sxc.Dnn;
using static ToSic.Lib.Code.Infos.CodeInfoObsolete;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav;

/// <summary>
/// The Eav DI Factory, used to construct various objects through [Dependency Injection](xref:NetCode.DependencyInjection.Index).
///
/// If possible avoid using this, as it's a workaround for code which is outside the normal Dependency Injection and therefor a bad pattern.
/// </summary>
[PrivateApi("Up to v19 used to PublicApi(Careful - obsolete!)")]
[Obsolete("Deprecated, please use Dnn 9 DI instead https://go.2sxc.org/brc-13-eav-factory")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class Factory
{
    [Obsolete("Not used any more, but keep for API consistency in case something calls ActivateNetCoreDi")]
    public delegate void ServiceConfigurator(IServiceCollection service);

    /// <summary>
    /// Dependency Injection resolver with a known type as a parameter.
    /// </summary>
    /// <typeparam name="T">The type / interface we need.</typeparam>
    [Obsolete("Please use standard Dnn 9.4+ Dnn DI instead https://go.2sxc.org/brc-13-eav-factory")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static T Resolve<T>()
    {
        DnnStaticDi.CodeInfos.Warn(WarnObsolete.UsedAs(specificId: typeof(T).FullName));
        return DnnStaticDi.StaticBuild<T>();
    }

    private static readonly ICodeInfo WarnObsolete = V13To17("ToSic.Eav.Factory.Resolve<T>", "https://go.2sxc.org/brc-13-eav-factory");
}
#endif