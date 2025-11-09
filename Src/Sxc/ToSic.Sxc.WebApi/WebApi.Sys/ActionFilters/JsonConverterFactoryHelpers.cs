using System.Text.Json;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Eav.WebApi.Sys.Helpers.Json;

namespace ToSic.Sxc.WebApi.Sys.ActionFilters;
internal class JsonConverterFactoryHelpers
{
    internal static TFormatter CreateNewFormatterFactory<TFormatter>(
        IServiceProvider sp,
        JsonFormatterAttribute? jsonFormatterAttribute,
        Func<Casing> getFallbackCasing,
        Func<JsonSerializerOptions, TFormatter> factory)
    {
        // Build Eav to Json converters for api v15
        // Important: Get the factory from the request-scoped service provider
        // This ensures it gets fresh IConvertToEavLight with the current request's culture from IZoneCultureResolver
        var eavJsonConverterFactory = NewEavJsonConverterFactoryOrNull(jsonFormatterAttribute?.EntityFormat, sp);

        var jsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtmlOptionsFactory(eavJsonConverterFactory);

        jsonSerializerOptions.SetCasing(jsonFormatterAttribute?.Casing ?? getFallbackCasing());

        return factory(jsonSerializerOptions);
    }

    /// <summary>
    /// Retrieves an instance of <see cref="entityFormat"/> based on the specified entity format.
    /// </summary>
    /// <param name="entityFormat">The desired entity format. </param>
    /// <param name="serviceProvider">The service provider used to resolve the factory and its dependencies.</param>
    /// <returns>An instance of <see cref="ToSic.Eav.WebApi.Sys.Helpers"/> if the entity format is <see langword="null"/> or <see
    /// cref="ToSic.Eav.WebApi.Sys.Helpers"/>; otherwise, <see langword="null"/>.</returns>
    public static EavJsonConverterFactory? NewEavJsonConverterFactoryOrNull(EntityFormat? entityFormat, IServiceProvider serviceProvider) =>
        entityFormat switch
        {
            // Build the factory from the request-scoped service provider
            // This is the key change - it ensures the factory and all its dependencies
            // (including IConvertToEavLight and IZoneCultureResolver) are request-scoped
            null or EntityFormat.Light => serviceProvider.Build<EavJsonConverterFactory>(),
            EntityFormat.None or _ => null,
        };
}
