using ToSic.Eav.Data;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Eav.WebApi.Sys.Helpers.Validation;

namespace ToSic.Sxc.Backend.SaveHelpers;

/// <summary>
/// Validates save packages for fields marked with <c>IsUnique</c> and reports collisions
/// against already persisted entities or other items in the same request.
/// </summary>
internal class IsUniqueValidator(ILog parentLog) : ValidatorBase(parentLog, "Val.UnqOk")
{
    private const string IsUniqueMetadataKey = "IsUnique";
    private const string InvariantLanguageBucket = "";
    private const string InvariantLanguageBucketLabel = "invariant";
    private static readonly UniqueValueKeyComparer UniqueKeyComparer = new();

    /// <summary>
    /// Checks all pending entities for duplicate values on unique fields and returns a bad-request
    /// exception when at least one conflict is found.
    /// </summary>
    internal HttpExceptionAbstraction? UniqueValuesOnly(IEnumerable<IEntity> existingEntities, IReadOnlyCollection<IEntity> pendingEntities)
    {
        var l = Log.Fn<HttpExceptionAbstraction?>($"{nameof(pendingEntities)}:{pendingEntities.Count}");

        if (pendingEntities.Count == 0)
            return l.ReturnNull("no pending entities");

        var existingByKey = IndexExistingEntries(existingEntities);
        var pendingByKey = GroupPendingEntries(pendingEntities);
        var errors = new List<string>();

        foreach (var pendingGroup in pendingByKey.Values)
            if (pendingGroup.Count > 1)
                AddPendingConflicts(errors, pendingGroup);
            else
            {
                var pendingEntry = pendingGroup[0];
                if (existingByKey.TryGetValue(pendingEntry.Key, out var existingEntry)
                    && existingEntry.LogicalId != pendingEntry.LogicalId)
                    errors.Add(FormatError(pendingEntry, existingEntry));
            }

        Errors = errors.Count == 0
            ? string.Empty
            : string.Join("\n", errors);

        var exception = BuildExceptionIfHasIssues(Errors, l, "UniqueValuesOnly() done");
        return l.Return(exception, exception == null ? "ok" : "duplicates");
    }

    /// <summary>
    /// Indexes existing entities by uniqueness key and keeps one representative entry per logical collision target.
    /// </summary>
    private Dictionary<UniqueValueKey, UniqueValueEntry> IndexExistingEntries(IEnumerable<IEntity> entities)
    {
        var existingByKey = new Dictionary<UniqueValueKey, UniqueValueEntry>(UniqueKeyComparer);

        foreach (var entry in EnumerateEntries(entities, UniqueValueSource.Existing))
        {
            if (!existingByKey.TryGetValue(entry.Key, out var existingEntry))
            {
                existingByKey.Add(entry.Key, entry);
                continue;
            }

            // Draft/published variants of the same logical entity should not count as duplicates.
            if (existingEntry.LogicalId == entry.LogicalId)
                continue;
        }

        return existingByKey;
    }

    /// <summary>
    /// Groups pending entities by uniqueness key while removing duplicate logical entities from the same request.
    /// </summary>
    private Dictionary<UniqueValueKey, List<UniqueValueEntry>> GroupPendingEntries(IEnumerable<IEntity> entities)
    {
        var pendingByKey = new Dictionary<UniqueValueKey, List<UniqueValueEntry>>(UniqueKeyComparer);

        foreach (var entry in EnumerateEntries(entities, UniqueValueSource.Pending))
        {
            if (!pendingByKey.TryGetValue(entry.Key, out var pendingGroup))
            {
                pendingByKey.Add(entry.Key, [entry]);
                continue;
            }

            if (ContainsLogicalId(pendingGroup, entry.LogicalId))
                continue;

            pendingGroup.Add(entry);
        }

        return pendingByKey;
    }

    /// <summary>
    /// Adds a validation error for every pending item in a duplicate pending group.
    /// </summary>
    private static void AddPendingConflicts(List<string> errors, List<UniqueValueEntry> pendingGroup)
    {
        if (pendingGroup.Count == 2)
        {
            errors.Add(FormatError(pendingGroup[0], pendingGroup[1]));
            errors.Add(FormatError(pendingGroup[1], pendingGroup[0]));
            return;
        }

        for (var i = 0; i < pendingGroup.Count; i++)
        {
            var conflict = i == 0
                ? pendingGroup[1]
                : pendingGroup[0];
            errors.Add(FormatError(pendingGroup[i], conflict));
        }
    }

    /// <summary>
    /// Projects entities into normalized uniqueness entries for each supported field value that participates
    /// in duplicate detection.
    /// </summary>
    private IEnumerable<UniqueValueEntry> EnumerateEntries(IEnumerable<IEntity> entities, UniqueValueSource source)
    {
        foreach (var (entity, index) in entities.Select((entity, index) => (entity, index)))
        {
            var logicalId = BuildLogicalId(entity, source, index);
            var contentTypeNameId = entity.Type.NameId;
            // Ignore repeated raw values inside the same logical entity. This keeps draft/published
            // variants or duplicate raw entries from creating false positives against themselves.
            var seenKeys = new HashSet<UniqueValueKey>(UniqueKeyComparer);

            foreach (var contentTypeAttribute in entity.Type.Attributes)
            {
                if (!contentTypeAttribute.Metadata.Get<bool>(IsUniqueMetadataKey)
                    || !IsSupportedType(contentTypeAttribute.Type))
                    continue;

                var entityAttribute = entity[contentTypeAttribute.Name];
                if (entityAttribute == null)
                    continue;

                var fieldName = contentTypeAttribute.Name;
                foreach (var rawValue in entityAttribute.Values)
                {
                    var normalized = NormalizeValue(contentTypeAttribute.Type, rawValue);
                    if (normalized == null)
                        continue;

                    var languageBucket = GetLanguageBucket(rawValue);
                    var key = new UniqueValueKey(
                        contentTypeNameId,
                        fieldName,
                        languageBucket,
                        normalized.Value.NormalizedKey
                    );

                    if (!seenKeys.Add(key))
                        continue;

                    yield return new(
                        key,
                        contentTypeNameId,
                        fieldName,
                        normalized.Value.DisplayValue,
                        languageBucket,
                        source,
                        logicalId,
                        entity,
                        index
                    );
                }
            }
        }
    }

    /// <summary>
    /// Checks whether a pending group already contains the same logical entity.
    /// </summary>
    private static bool ContainsLogicalId(List<UniqueValueEntry> pendingGroup, string logicalId)
    {
        foreach (var existingEntry in pendingGroup)
            if (existingEntry.LogicalId == logicalId)
                return true;

        return false;
    }

    /// <summary>
    /// Restricts uniqueness checks to the scalar field types supported by the first implementation
    /// and explicitly skips relationship or unstructured value types.
    /// </summary>
    private static bool IsSupportedType(ValueTypes type)
        => type switch
        {
            ValueTypes.String => true,
            ValueTypes.Hyperlink => true,
            ValueTypes.Custom => true,
            ValueTypes.Number => true,
            ValueTypes.Boolean => true,
            ValueTypes.DateTime => true,
            ValueTypes.Entity => false,
            ValueTypes.Empty => false,
            ValueTypes.Object => false,
            ValueTypes.Json => false,
            ValueTypes.Undefined => false,
            _ => false
        };

    /// <summary>
    /// Normalizes a raw attribute value into a comparison key and display value, or returns null
    /// when the value should not participate in uniqueness checks.
    /// </summary>
    private static NormalizedValue? NormalizeValue(ValueTypes type, IValue rawValue)
    {
        if (type is ValueTypes.String or ValueTypes.Hyperlink or ValueTypes.Custom)
        {
            // Text uniqueness is intentionally trim + case-insensitive; blank text is treated as unset.
            var stringValue = rawValue.ObjectContents as string ?? rawValue.SerializableObject as string ?? rawValue.Serialized;
            if (stringValue == null)
                return null;

            var trimmed = stringValue.Trim();
            if (trimmed.Length == 0)
                return null;

            return new(trimmed, trimmed);
        }

        // Non-string scalar types compare on their invariant serialized form.
        var serialized = rawValue.Serialized;
        if (string.IsNullOrWhiteSpace(serialized))
            return null;

        return new(serialized!, serialized!);
    }

    /// <summary>
    /// Builds the exact language bucket used for uniqueness comparisons so invariant and translated
    /// values are checked within their own scope.
    /// </summary>
    private static string GetLanguageBucket(IValue rawValue)
    {
        // Uniqueness is scoped to the exact language bucket of the raw value.
        // Invariant values only conflict with other invariant values.
        string? firstLanguage = null;
        List<string>? languages = null;
        HashSet<string>? seen = null;

        foreach (var language in rawValue.Languages)
        {
            var key = language.Key;
            if (string.IsNullOrWhiteSpace(key))
                continue;

            if (firstLanguage == null)
            {
                firstLanguage = key;
                continue;
            }

            if (languages == null)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(firstLanguage, key))
                    continue;

                languages = [firstLanguage, key];
                seen = new(StringComparer.OrdinalIgnoreCase) { firstLanguage, key };
                continue;
            }

            if (seen!.Add(key))
                languages!.Add(key);
        }

        if (firstLanguage == null)
            return InvariantLanguageBucket;

        if (languages == null)
            return firstLanguage;

        languages.Sort(StringComparer.OrdinalIgnoreCase);
        return string.Join("|", languages);
    }

    /// <summary>
    /// Resolves a stable logical identifier for an entity so saved and pending variants of the same
    /// item do not conflict with themselves.
    /// </summary>
    private static string BuildLogicalId(IEntity entity, UniqueValueSource source, int index)
    {
        // Prefer stable logical IDs so an edited entity does not collide with its already-saved version.
        // New items can still have Guid.Empty, so pending entries fall back to their request index.
        if (entity.EntityGuid != Guid.Empty)
            return "g:" + entity.EntityGuid;

        if (source == UniqueValueSource.Pending)
            return "p:" + index;

        if (entity.RepositoryId > 0)
            return "r:" + entity.RepositoryId;

        return $"e:{entity.EntityId}:{index}";
    }

    /// <summary>
    /// Formats the user-facing validation message for a detected uniqueness conflict.
    /// </summary>
    private static string FormatError(UniqueValueEntry pendingEntry, UniqueValueEntry conflict)
    {
        var languageLabel = pendingEntry.LanguageBucket == InvariantLanguageBucket
            ? InvariantLanguageBucketLabel
            : pendingEntry.LanguageBucket;

        var conflictDescription = conflict.Source == UniqueValueSource.Pending
            ? $"another item in the same request (item {conflict.Index})"
            : $"saved entity {conflict.Entity.EntityId}";

        return $"Duplicate unique value in {pendingEntry.ContentTypeNameId}.{pendingEntry.FieldName} [{languageLabel}]: '{pendingEntry.DisplayValue}' already exists on {conflictDescription}.";
    }

    private readonly record struct UniqueValueKey(string ContentTypeNameId, string FieldName, string LanguageBucket, string NormalizedValue);

    private readonly record struct UniqueValueEntry(
        UniqueValueKey Key,
        string ContentTypeNameId,
        string FieldName,
        string DisplayValue,
        string LanguageBucket,
        UniqueValueSource Source,
        string LogicalId,
        IEntity Entity,
        int Index
    );

    private readonly record struct NormalizedValue(string NormalizedKey, string DisplayValue);

    private sealed class UniqueValueKeyComparer : IEqualityComparer<UniqueValueKey>
    {
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

        public bool Equals(UniqueValueKey x, UniqueValueKey y)
            => Comparer.Equals(x.ContentTypeNameId, y.ContentTypeNameId)
               && Comparer.Equals(x.FieldName, y.FieldName)
               && Comparer.Equals(x.LanguageBucket, y.LanguageBucket)
               && Comparer.Equals(x.NormalizedValue, y.NormalizedValue);

        public int GetHashCode(UniqueValueKey obj)
        {
            var hash = new HashCode();
            hash.Add(obj.ContentTypeNameId, Comparer);
            hash.Add(obj.FieldName, Comparer);
            hash.Add(obj.LanguageBucket, Comparer);
            hash.Add(obj.NormalizedValue, Comparer);
            return hash.ToHashCode();
        }
    }

    private enum UniqueValueSource
    {
        Existing,
        Pending
    }
}
