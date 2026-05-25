using ToSic.Eav.DataSource;
using ToSic.Eav.WebApi.Sys.Helpers.Validation;

namespace ToSic.Sxc.Backend.SaveHelpers;

/// <summary>
/// Validates save packages for fields marked with <c>IsUnique</c>.
/// </summary>
internal class IsUniqueValidator(UniqueValueLookup lookup, IDataSource appData, ILog parentLog) : ValidatorBase(parentLog, "Val.UnqOk")
{
    private const string IsUniqueMetadataKey = "IsUnique";
    private const string StringUrlPathInputType = "string-url-path";
    private const string InvariantLanguage = "";

    internal HttpExceptionAbstraction? UniqueValuesOnly(IReadOnlyCollection<IEntity> pendingEntities)
    {
        var l = Log.Fn<HttpExceptionAbstraction?>($"{nameof(pendingEntities)}:{pendingEntities.Count}", timer: true);
        
        if (pendingEntities.Count == 0)
            return l.ReturnNull("no pending entities");

        // Materialize once so we can first detect duplicates inside the same save package,
        // then reuse the same normalized values for persisted-entity lookups.
        var pending = PendingValues(pendingEntities).ToList();

        if (FindSameRequestConflict(pending) is { } sameRequest)
            return DuplicateException(l, sameRequest.Entry, sameRequest.Conflict, sameRequest.ConflictIndex, "same request");

        foreach (var entry in pending)
            if (lookup.FindConflict(appData, LookupRequest(entry)) is { } conflict)
                return DuplicateException(l, entry, conflict, null, "existing");

        Errors = string.Empty;
        var exception = BuildExceptionIfHasIssues(Errors, l, "UniqueValuesOnly() done");
        return l.Return(exception, "ok");
    }

    private IEnumerable<PendingValue> PendingValues(IEnumerable<IEntity> pendingEntities)
    {
        var index = 0;

        foreach (var entity in pendingEntities)
        {
            foreach (var pendingValue in PendingValues(entity, index))
                yield return pendingValue;

            index++;
        }
    }

    private IEnumerable<PendingValue> PendingValues(IEntity entity, int index)
    {
        // Flatten the entity into comparable values so request-local and persisted checks
        // use the exact same normalization and identity data.
        foreach (var field in UniqueFields(entity.Type))
            if (entity[field.Name] is { } attribute)
                foreach (var raw in attribute.Values)
                    if (NormalizedValue(field.Type, raw) is { } value)
                    {
                        var language = LanguageKey(raw);
                        yield return new(entity.Type.NameId, field.Name, field.Type, value, language, PendingValueKey(entity.Type.NameId, field.Name, language, value), entity, index);
                    }
    }

    private static IContentTypeAttribute[] UniqueFields(IContentType contentType)
        // Url-path fields are unique by default unless metadata explicitly overrides that behavior.
        => contentType.Attributes
            .Where(attribute => UniqueValueLookup.IsSupported(attribute.Type)
                                && (attribute.Metadata.Get<bool?>(IsUniqueMetadataKey) ?? IsUrlPath(attribute)))
            .ToArray();

    private static bool IsUrlPath(IContentTypeAttribute attribute)
        => attribute.Type == ValueTypes.String
           && attribute.InputType?.Equals(StringUrlPathInputType, StringComparison.OrdinalIgnoreCase) == true;

    private static string? NormalizedValue(ValueTypes type, IValue raw)
    {
        var value = type is ValueTypes.String or ValueTypes.Hyperlink or ValueTypes.Custom
            ? raw.ObjectContents as string ?? raw.SerializableObject as string ?? raw.Serialized
            : raw.Serialized;

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static string LanguageKey(IValue raw)
    {
        var languages = raw.Languages
            .Select(language => language.Key)
            .Where(language => !string.IsNullOrWhiteSpace(language))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(language => language, StringComparer.OrdinalIgnoreCase);

        return string.Join("|", languages);
    }

    private static string PendingValueKey(string type, string field, string language, string value)
        => string.Join("\n", type, field, language, value).ToLowerInvariant();

    private static UniqueValueLookupRequest LookupRequest(PendingValue entry)
        // The invariant bucket is represented as null for datasource filtering.
        => new(
            entry.ContentType,
            entry.Field,
            entry.Type,
            entry.Value,
            CurrentEntity: entry.Entity,
            Languages: entry.Language == InvariantLanguage ? null : entry.Language
        );

    private static (PendingValue Entry, IEntity Conflict, int ConflictIndex)? FindSameRequestConflict(IEnumerable<PendingValue> pending)
    {
        // Reuse the same composite key shape as the persisted lookup so request-local duplicates
        // are detected before we hit the datasource.
        var seen = new Dictionary<string, PendingValue>(StringComparer.Ordinal);
        foreach (var entry in pending)
        {
            if (!seen.TryGetValue(entry.Key, out var existing))
            {
                seen.Add(entry.Key, entry);
                continue;
            }

            if (!UniqueValueLookup.IsSameEntity(existing.Entity, entry.Entity))
                return (entry, existing.Entity, existing.Index);
        }

        return null;
    }

    private HttpExceptionAbstraction? DuplicateException(ILogCall<HttpExceptionAbstraction?>? l, PendingValue entry, IEntity conflict, int? requestIndex, string source)
    {
        var language = entry.Language == InvariantLanguage ? "invariant" : entry.Language;
        var target = requestIndex.HasValue
            ? $"another item in the same request (item {requestIndex.Value})"
            : $"saved entity {conflict.EntityId}";

        // Keep the message and log in one place so request-local and persisted conflicts explain
        // duplicates in the same format.
        Errors = $"Duplicate unique value in {entry.ContentType}.{entry.Field}: value '{entry.Value}' (language: {language}) already exists on {target}.";
        Log.A($"Unique conflict ct:{entry.ContentType} field:{entry.Field} value:'{entry.Value}' lang:{language} " +
              $"pending[item:{entry.Index}, entityId:{entry.Entity.EntityId}, repoId:{entry.Entity.RepositoryId}, guid:{entry.Entity.EntityGuid}] " +
              $"conflict[entityId:{conflict.EntityId}, repoId:{conflict.RepositoryId}, guid:{conflict.EntityGuid}]");

        var exception = BuildExceptionIfHasIssues(Errors, l, "UniqueValuesOnly() done");
        return l.Return(exception, $"duplicate:first; source:{source}");
    }

    private readonly record struct PendingValue(
        string ContentType,
        string Field,
        ValueTypes Type,
        string Value,
        string Language,
        string Key,
        IEntity Entity,
        int Index
    );
}
