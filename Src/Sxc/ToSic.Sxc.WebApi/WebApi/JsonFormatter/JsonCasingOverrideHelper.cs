using ToSic.Sys.Utils;

namespace ToSic.Sxc.WebApi;

/// <summary>
/// Shared helpers for request-level JSON casing overrides.
/// Framework-specific code should pass query key/value pairs into this utility.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal static class JsonCasingOverrideHelper
{
    internal const string CasingQueryKey = "$casing";
    private const string CamelValue = "camel";

    /// <summary>
    /// Parse casing override from query parameters.
    /// Currently, supports only `$casing=camel`.
    /// </summary>
    internal static bool TryParseCasingOverride(IEnumerable<KeyValuePair<string, string>>? queryNameValuePairs, out Casing casing)
    {
        casing = Casing.Unspecified;
        if (queryNameValuePairs == null)
            return false;

        var casingQueryValue = queryNameValuePairs
            .FirstOrDefault(pair => pair.Key.EqualsInsensitive(CasingQueryKey))
            .Value;

        if (!casingQueryValue.EqualsInsensitive(CamelValue))
            return false;

        casing = Casing.Camel;
        return true;
    }
}
