using System.Globalization;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.Data.Sys;

namespace ToSic.Sxc.Backend.SaveHelpers;

internal static class UniqueValueValidationRules
{
    internal const string IsUniqueMetadataKey = "IsUnique";
    internal const string StringUrlPathInputType = "string-url-path";
    internal const string InvariantLanguage = "";
    private static readonly ValueAssembler ScalarValueAssembler = new();

    internal static IContentTypeAttribute[] UniqueFields(IContentType contentType)
        // Url-path fields are unique by default unless metadata explicitly overrides that behavior.
        => contentType.Attributes
            .Where(IsUniqueField)
            .ToArray();

    internal static bool IsUniqueField(IContentTypeAttribute attribute)
        => UniqueValueLookup.IsSupported(attribute.Type)
           && (attribute.Metadata.Get<bool?>(IsUniqueMetadataKey) ?? IsUrlPath(attribute));

    internal static bool IsUrlPath(IContentTypeAttribute attribute)
        => attribute.Type == ValueTypes.String
           && attribute.InputType.Equals(StringUrlPathInputType, StringComparison.OrdinalIgnoreCase);

    internal static string? NormalizedValue(ValueTypes type, IValue raw)
    {
        var value = type is ValueTypes.String or ValueTypes.Hyperlink or ValueTypes.Custom
            ? raw.ObjectContents as string ?? raw.SerializableObject as string ?? raw.Serialized
            : raw.Serialized;

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    internal static string? NormalizedValue(ValueTypes type, string? value)
    {
        var normalizedValue = value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedValue))
            return null;

        return type switch
        {
            ValueTypes.DateTime => NormalizeDateTimeValue(normalizedValue),
            _ => normalizedValue,
        };
    }

    private static string NormalizeDateTimeValue(string value)
    {
        // Edit UI sends ISO UTC strings like 2026-05-21T00:00:00.000Z, but uniqueness lookup must compare
        // against EAV's short DateTime serialization without shifting the wall-clock value through local time.
        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
            return ScalarValueAssembler.DateTime(dto.DateTime, DataConstants.NoLanguages).Serialized ?? value;

        return ScalarValueAssembler.Create(ValueTypes.DateTime, value).Serialized ?? value;
    }

    internal static string LanguageKey(IValue raw)
    {
        var languages = raw.Languages
            .Select(language => language.Key)
            .Where(language => !string.IsNullOrWhiteSpace(language))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(language => language, StringComparer.OrdinalIgnoreCase);

        return string.Join("|", languages);
    }

    internal static string? LanguageFilterOrNull(string language)
        => language == InvariantLanguage
            ? null
            : language;
}