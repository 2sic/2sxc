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
    LazySvc<IDataFactory> DataFactory,
    LazySvc<IUser> User)
    : DependenciesRecord(connect: [ValueConverter, Scrub, ForCode, DataFactory])
{
    ///// <summary>
    ///// The ValueConverter is used to parse links in the format like "file:72"
    ///// </summary>
    ///// <remarks>
    ///// Before 2025-06-18 was called ValueConverterOrNull - but it's from a service so it should always be there...
    ///// </remarks>
    //internal IValueConverter ValueConverter => ValueConverterLazy.Value;

    ///// <summary>
    ///// This is provided so that ITypedItems can use Scrub in the String APIs
    ///// </summary>
    //internal LazySvc<IScrub> Scrub => Scrub;

    //internal LazySvc<ConvertForCodeService> ForCode => ForCode;

    //internal LazySvc<IDataFactory> DataFactory => DataFactory;

    ///// <summary>
    ///// User information for detecting if draft data is allowed.
    ///// </summary>
    //internal LazySvc<IUser> User => UserLazy;
}