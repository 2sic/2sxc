using System.Globalization;

namespace ToSic.Sxc.Backend.SaveHelpers;

internal static class UniqueValueValidationRules
{
    internal const string IsUniqueMetadataKey = "IsUnique";
    internal const string StringUrlPathInputType = "string-url-path";
    internal const string InvariantLanguage = "";
    internal const string DefaultLanguageWildcard = "*";

    internal static IContentTypeField[] UniqueFields(IContentType contentType)
        // Url-path fields are unique by default unless metadata explicitly overrides that behavior.
        => contentType.Attributes
            .Where(IsUniqueField)
            .ToArray();

    internal static bool IsUniqueField(IContentTypeField fieldDef)
        => UniqueValueLookup.IsSupported(fieldDef.Type)
           && (fieldDef.Metadata.Get<bool?>(IsUniqueMetadataKey) ?? IsUrlPath(fieldDef));

    internal static bool IsUrlPath(IContentTypeField fieldDef)
        => fieldDef.Type == ValueTypes.String
           && fieldDef.InputType.Equals(StringUrlPathInputType, StringComparison.OrdinalIgnoreCase);

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
        return string.IsNullOrWhiteSpace(value)
            ? null
            : type switch
            {
                ValueTypes.DateTime => NormalizeDateTimeValue(value!),
                _ => value,
            };
    }

    private static string NormalizeDateTimeValue(string value)
    {
        const string dateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF";

        // ValueFilter compares the stored wall-clock time. Strip the ISO offset before it parses the value.
        return DateTimeOffset.TryParseExact(value, dateTimeFormat + "K", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date.DateTime.ToString(dateTimeFormat, CultureInfo.InvariantCulture)
            : value;
    }

    internal static string LanguageKey(IValue raw)
    {
        var languages = raw.Languages
            .Select(language => NormalizeLanguage(language.Key))
            .Where(language => !string.IsNullOrWhiteSpace(language))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(language => language, StringComparer.OrdinalIgnoreCase);

        return string.Join("|", languages);
    }

    internal static string? LanguageFilterOrNull(string language)
    {
        var normalized = NormalizeLanguage(language);
        return normalized == InvariantLanguage
            ? null
            : normalized;
    }

    private static string NormalizeLanguage(string? language)
        // EAV JSON uses "*" as the persisted marker for the default/non-localized bucket.
        => language == DefaultLanguageWildcard
            ? InvariantLanguage
            : language ?? InvariantLanguage;
}
