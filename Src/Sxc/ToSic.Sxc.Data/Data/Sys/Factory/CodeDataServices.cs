using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Sys.ValueConverter;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services.Sys.ConvertService;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Data.Sys.Factory;

/// <summary>
/// Helper services published by the CodeDataFactory for use in certain other objects which depend on it.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record CodeDataServices(
    LazySvc<IValueConverter> ValueConverter,
    LazySvc<IScrub> Scrub,
    LazySvc<ConvertForCodeService> ForCode,
    Generator<IDataFactory, DataFactoryOptions> DataFactory,
    LazySvc<IUser> User
)
    : DependenciesBase(connect: [ValueConverter, Scrub, ForCode, DataFactory]);