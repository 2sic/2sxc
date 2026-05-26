namespace ToSic.Sxc.Backend.SaveHelpers;

internal static class UniqueValueValidationRules
{
    internal const string IsUniqueMetadataKey = "IsUnique";
    internal const string StringUrlPathInputType = "string-url-path";
    internal const string InvariantLanguage = "";

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
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value;

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