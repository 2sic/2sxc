using System.Globalization;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.Data.Sys;

namespace ToSic.Sxc.Backend.SaveHelpers;

internal static class UniqueValueValidationRules
{
    internal const string IsUniqueMetadataKey = "IsUnique";
    internal const string StringUrlPathInputType = "string-url-path";
    internal const string InvariantLanguage = "";
    internal const string DefaultLanguageWildcard = "*";
    
    // 2026-08-07 2dm TODO: @STV - THIS IS NOT MEANT FOR static use
    //private static readonly ValueAssembler ScalarValueAssembler = new();

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
        // 2026-08-07 2dm TODO: @STV - THIS IS NOT MEANT FOR static use
        // TODO: pls recheck, and make something simpler, this feels like overkill
        return value;
        
        //// Edit UI sends ISO UTC strings like 2026-05-21T00:00:00.000Z, but uniqueness lookup must compare
        //// against EAV's short DateTime serialization without shifting the wall-clock value through local time.
        //if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
        //    return ScalarValueAssembler.DateTime(dto.DateTime, DataConstants.NoLanguages).Serialized ?? value;

        //return ScalarValueAssembler.Create(ValueTypes.DateTime, value).Serialized ?? value;
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
