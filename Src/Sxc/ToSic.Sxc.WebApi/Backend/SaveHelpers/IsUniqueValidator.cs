using System.Collections;
using ToSic.Eav.Data.Sys.Entities.Sources;
using ToSic.Eav.Data.Sys.Relationships;
using ToSic.Eav.Sys;
using static ToSic.Eav.WebApi.Sys.Helpers.Validation.ValidatorBase;

namespace ToSic.Sxc.Backend.SaveHelpers;

/// <summary>
/// Validates save-time uniqueness for fields marked with <c>IsUnique</c>.
/// The current save pipeline validates one pending entity at a time against already persisted data.
/// </summary>
[PrivateApi]
internal class IsUniqueValidator : ServiceBase, ISaveEntityValidator
{
    private const string IsUniqueMetadataKey = "IsUnique";
    private const string InvariantLanguageBucket = "";
    private const string InvariantLanguageBucketLabel = "invariant";
    private static readonly UniqueValueKeyComparer UniqueKeyComparer = new();

    public IsUniqueValidator()
        : base("Val.UnqOk")
    { }

    internal IsUniqueValidator(ILog parentLog)
        : this()
        => ConnectLogs([parentLog]);

    SaveEntityValidationResult ISaveEntityValidator.Validate(SaveEntityValidationContext context)
        => new(UniqueValueOnly(context.ExistingEntities, context.Entity, context.Index));

    /// <summary>
    /// Checks one pending entity for duplicate values on unique fields and returns a bad-request
    /// exception when at least one persisted collision is found.
    /// </summary>
    internal HttpExceptionAbstraction? UniqueValueOnly(IEnumerable<IEntity> existingEntities, IEntity pendingEntity, int index = 0)
    {
        var l = Log.Fn<HttpExceptionAbstraction?>($"index:{index}");

        var existingByKey = IndexExistingEntries(existingEntities);
        var errors = new List<string>();
        var duplicateCount = AddExistingConflicts(errors, existingByKey, EnumerateEntries([pendingEntity], UniqueValueSource.Pending, index));
        var errorText = errors.Count == 0
            ? string.Empty
            : string.Join("\n", errors);

        var exception = BuildExceptionIfHasIssues(errorText, l, "UniqueValueOnly() done");
        return l.Return(exception, exception == null ? "ok" : $"duplicates:{duplicateCount}");
    }

    /// <summary>
    /// Compatibility shim which validates each pending entity independently against existing data.
    /// Duplicate values inside the same save package are intentionally ignored by this refactored flow.
    /// </summary>
    internal HttpExceptionAbstraction? UniqueValuesOnly(IEnumerable<IEntity> existingEntities, IReadOnlyCollection<IEntity> pendingEntities)
    {
        var l = Log.Fn<HttpExceptionAbstraction?>($"{nameof(pendingEntities)}:{pendingEntities.Count}");

        if (pendingEntities.Count == 0)
            return l.ReturnNull("no pending entities");

        var existingByKey = IndexExistingEntries(existingEntities);
        var errors = new List<string>();
        var duplicateCount = 0;

        foreach (var (pendingEntity, index) in pendingEntities.Select((entity, index) => (entity, index)))
            duplicateCount += AddExistingConflicts(errors, existingByKey, EnumerateEntries([pendingEntity], UniqueValueSource.Pending, index));

        var errorText = errors.Count == 0
            ? string.Empty
            : string.Join("\n", errors);

        var exception = BuildExceptionIfHasIssues(errorText, l, "UniqueValuesOnly() done");
        return l.Return(exception, exception == null ? "ok" : $"duplicates:{duplicateCount}");
    }

    /// <summary>
    /// Indexes existing entities by uniqueness key and keeps one representative entry per logical collision target.
    /// </summary>
    private Dictionary<UniqueValueKey, UniqueValueEntry> IndexExistingEntries(IEnumerable<IEntity> entities)
    {
        var existingByKey = new Dictionary<UniqueValueKey, UniqueValueEntry>(UniqueKeyComparer);

        // A single representative per unique key is enough for existing data.
        // Additional entries are ignored, including draft/published variants
        // of the same logical entity.
        foreach (var entry in EnumerateEntries(entities, UniqueValueSource.Existing))
            if (!existingByKey.ContainsKey(entry.Key))
                existingByKey.Add(entry.Key, entry);

        return existingByKey;
    }

    /// <summary>
    /// Adds a formatted conflict to the validation result and to the diagnostics log.
    /// </summary>
    private int AddConflict(List<string> errors, UniqueValueEntry pendingEntry, UniqueValueEntry conflict)
    {
        errors.Add(FormatError(pendingEntry, conflict));
        Log.A(FormatLogConflict(pendingEntry, conflict));
        return 1;
    }

    /// <summary>
    /// Adds a conflict when the pending entry matches an already persisted item with a different logical identity.
    /// </summary>
    private int AddExistingConflictIfAny(List<string> errors, IReadOnlyDictionary<UniqueValueKey, UniqueValueEntry> existingByKey, UniqueValueEntry pendingEntry)
    {
        if (!existingByKey.TryGetValue(pendingEntry.Key, out var existingEntry)
            || existingEntry.LogicalId == pendingEntry.LogicalId)
            return 0;

        return AddConflict(errors, pendingEntry, existingEntry);
    }

    /// <summary>
    /// Adds persisted conflicts for the supplied pending entries.
    /// </summary>
    private int AddExistingConflicts(List<string> errors, IReadOnlyDictionary<UniqueValueKey, UniqueValueEntry> existingByKey, IEnumerable<UniqueValueEntry> pendingEntries)
    {
        var count = 0;

        foreach (var pendingEntry in pendingEntries)
            count += AddExistingConflictIfAny(errors, existingByKey, pendingEntry);

        return count;
    }

    /// <summary>
    /// Projects entities into normalized uniqueness entries for each supported field value that participates
    /// in duplicate detection.
    /// </summary>
    private IEnumerable<UniqueValueEntry> EnumerateEntries(IEnumerable<IEntity> entities, UniqueValueSource source, int startIndex = 0)
    {
        var uniqueFieldsByType = new Dictionary<string, IContentTypeAttribute[]>(StringComparer.OrdinalIgnoreCase);
        var index = startIndex;
        foreach (var entity in entities)
        {
            var logicalId = BuildLogicalId(entity, source, index);
            var contentTypeNameId = entity.Type.NameId;
            var uniqueFields = GetUniqueFields(entity.Type, contentTypeNameId, uniqueFieldsByType);
            if (uniqueFields.Length == 0)
            {
                index++;
                continue;
            }

            // Ignore repeated raw values inside the same logical entity. This keeps draft/published
            // variants or duplicate raw entries from creating false positives against themselves.
            var seenKeys = new HashSet<UniqueValueKey>(UniqueKeyComparer);

            foreach (var contentTypeAttribute in uniqueFields)
            {
                var entityAttribute = entity[contentTypeAttribute.Name];
                if (entityAttribute == null)
                    continue;

                var fieldName = contentTypeAttribute.Name;
                foreach (var rawValue in entityAttribute.Values)
                {
                    var languageBucket = GetLanguageBucket(rawValue);
                    foreach (var normalized in NormalizeValues(contentTypeAttribute.Type, rawValue))
                    {
                        var key = new UniqueValueKey(
                            contentTypeNameId,
                            fieldName,
                            languageBucket,
                            normalized.NormalizedKey
                        );

                        if (!seenKeys.Add(key))
                            continue;

                        yield return new(
                            key,
                            contentTypeNameId,
                            fieldName,
                            normalized.DisplayValue,
                            languageBucket,
                            source,
                            logicalId,
                            entity,
                            index
                        );
                    }
                }
            }

            index++;
        }
    }

    /// <summary>
    /// Retrieves and caches the fields which participate in unique validation for a content type.
    /// </summary>
    private static IContentTypeAttribute[] GetUniqueFields(IContentType contentType, string contentTypeNameId, IDictionary<string, IContentTypeAttribute[]> cache)
    {
        if (cache.TryGetValue(contentTypeNameId, out var uniqueFields))
            return uniqueFields;

        List<IContentTypeAttribute>? fields = null;
        foreach (var attribute in contentType.Attributes)
        {
            if (!IsSupportedType(attribute.Type) || !attribute.Metadata.Get<bool>(IsUniqueMetadataKey))
                continue;

            (fields ??= []).Add(attribute);
        }

        uniqueFields = fields?.ToArray() ?? [];
        cache[contentTypeNameId] = uniqueFields;
        return uniqueFields;
    }

    /// <summary>
    /// Restricts uniqueness checks to the field types supported by the current implementation.
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
            ValueTypes.Entity => true,
            _ => false
        };

    /// <summary>
    /// Normalizes a raw attribute value into one or more comparison keys.
    /// Entity fields yield one normalized item per related child, because the DB stores them
    /// as separate relationship rows.
    /// </summary>
    private static IEnumerable<NormalizedValue> NormalizeValues(ValueTypes type, IValue rawValue)
    {
        if (type == ValueTypes.Entity)
            foreach (var normalized in NormalizeEntityValues(rawValue))
                yield return normalized;

        else if ((type is ValueTypes.String or ValueTypes.Hyperlink or ValueTypes.Custom
            ? GetRawStringValue(rawValue)?.Trim()
            : rawValue.Serialized) is { Length: > 0 } normalizedValue)
            yield return new(normalizedValue, normalizedValue);
    }

    /// <summary>
    /// Normalizes relationship values one child at a time using child entity guids whenever possible.
    /// </summary>
    private static IEnumerable<NormalizedValue> NormalizeEntityValues(IValue rawValue)
    {
        var foundGuidValue = false;
        foreach (var normalized in BuildEntityNormalizedValues(TryGetRelationshipGuids(rawValue)))
        {
            foundGuidValue = true;
            yield return normalized;
        }

        if (foundGuidValue)
            yield break;

        foreach (var normalized in BuildEntityNormalizedValues(TryGetRelationshipIdentifiers(rawValue)))
            yield return normalized;
    }

    /// <summary>
    /// Tries to resolve relationship references to entity guids so persisted int-based relationships
    /// can be compared with pending guid-based relationships before the save pipeline attaches the app cache.
    /// </summary>
    private static IEnumerable? TryGetRelationshipGuids(IValue rawValue)
    {
        if (rawValue.ObjectContents is LazyEntitiesSource lazyEntities)
            try
            {
                return lazyEntities.ResolveGuids();
            }
            catch
            {
                // Fall back to raw identifiers when the lazy relationship does not have a lookup source.
            }

        return AsNonStringEnumerable(rawValue.SerializableObject);
    }

    /// <summary>
    /// Falls back to the raw relationship identifiers when guid resolution is unavailable.
    /// </summary>
    private static IEnumerable? TryGetRelationshipIdentifiers(IValue rawValue)
    {
        if (rawValue.ObjectContents is IRelatedEntitiesValue relatedEntities)
            return relatedEntities.Identifiers;

        return AsNonStringEnumerable(rawValue.ObjectContents);
    }

    /// <summary>
    /// Retrieves the raw textual representation used for uniqueness comparisons on text-like fields.
    /// </summary>
    private static string? GetRawStringValue(IValue rawValue)
        => rawValue.ObjectContents as string
           ?? rawValue.SerializableObject as string
           ?? rawValue.Serialized;

    /// <summary>
    /// Returns an enumerable when the supplied object is a collection-like value but not a string.
    /// </summary>
    private static IEnumerable? AsNonStringEnumerable(object? value)
        => value is IEnumerable enumerable and not string
            ? enumerable
            : null;

    /// <summary>
    /// Creates the canonical comparison token for each child in a relationship value.
    /// </summary>
    private static IEnumerable<NormalizedValue> BuildEntityNormalizedValues(IEnumerable? identifiers)
    {
        if (identifiers == null)
            yield break;

        foreach (var identifier in identifiers)
            if (NormalizeEntityIdentifier(identifier) is { } token)
                yield return new(token, token);
    }

    /// <summary>
    /// Converts a relationship identifier into the canonical token used by raw EAV serialization.
    /// </summary>
    private static string? NormalizeEntityIdentifier(object? identifier)
    {
        switch (identifier)
        {
            case null:
                return null;

            case Guid guid when guid == Guid.Empty:
                return null;

            case Guid guid:
                return guid.ToString();

            case IEntity entity when entity.EntityGuid != Guid.Empty:
                return entity.EntityGuid.ToString();

            case IEntity { RepositoryId: > 0 } entity:
                return entity.RepositoryId.ToString();

            case IEntity { EntityId: > 0 } entity:
                return entity.EntityId.ToString();

            case string text when string.IsNullOrWhiteSpace(text) || text.Equals(EavConstants.EmptyRelationship, StringComparison.OrdinalIgnoreCase):
                return null;

            case string text:
                return text.Trim();

            default:
                var value = identifier.ToString();
                return string.IsNullOrWhiteSpace(value)
                    ? null
                    : value;
        }
    }

    /// <summary>
    /// Builds the exact language bucket used for uniqueness comparisons so invariant and translated
    /// values are checked within their own scope.
    /// </summary>
    private static string GetLanguageBucket(IValue rawValue)
    {
        var languages = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var language in rawValue.Languages)
            if (!string.IsNullOrWhiteSpace(language.Key))
                languages.Add(language.Key);

        return languages.Count switch
        {
            0 => InvariantLanguageBucket,
            1 => languages.Min!,
            _ => string.Join("|", languages)
        };
    }

    /// <summary>
    /// Resolves the compact logical identity used to decide whether two uniqueness entries represent
    /// the same real-world item.
    /// </summary>
    /// <remarks>
    /// The method prefers identifiers that survive round-trips between persisted and pending states.
    /// When that is impossible, it falls back step by step to request-local values that are still
    /// stable enough to suppress self-collisions inside the current validation pass.
    /// </remarks>
    private static LogicalId BuildLogicalId(IEntity entity, UniqueValueSource source, int index)
    {
        // Prefer stable logical IDs so an edited entity does not collide with its already-saved version.
        // New items can still have Guid.Empty, so pending entries fall back to their request index.
        if (entity.EntityGuid != Guid.Empty)
            return new(LogicalIdKind.Guid, entity.EntityGuid, 0, 0);

        if (source == UniqueValueSource.Pending)
            return new(LogicalIdKind.PendingIndex, Guid.Empty, index, 0);

        if (entity.RepositoryId > 0)
            return new(LogicalIdKind.RepositoryId, Guid.Empty, entity.RepositoryId, 0);

        return new(LogicalIdKind.EntityIdAndIndex, Guid.Empty, entity.EntityId, index);
    }

    /// <summary>
    /// Formats the user-facing validation message for a detected uniqueness conflict.
    /// </summary>
    private static string FormatError(UniqueValueEntry pendingEntry, UniqueValueEntry conflict)
    {
        var languageLabel = FormatLanguageLabel(pendingEntry.LanguageBucket);
        var conflictDescription = DescribeConflictTarget(conflict);

        return $"Duplicate unique value in {pendingEntry.ContentTypeNameId}.{pendingEntry.FieldName} [{languageLabel}]: '{pendingEntry.DisplayValue}' already exists on {conflictDescription}.";
    }

    /// <summary>
    /// Formats a log line with both the duplicate value and the entity identifiers involved.
    /// </summary>
    private static string FormatLogConflict(UniqueValueEntry pendingEntry, UniqueValueEntry conflict)
    {
        var languageLabel = FormatLanguageLabel(pendingEntry.LanguageBucket);

        return $"Unique conflict ct:{pendingEntry.ContentTypeNameId} field:{pendingEntry.FieldName} lang:{languageLabel} value:'{pendingEntry.DisplayValue}' " +
               $"pending[{DescribeEntity(pendingEntry)}] conflict[{DescribeEntity(conflict)}]";
    }

    /// <summary>
    /// Formats the language bucket for human-readable diagnostics.
    /// </summary>
    private static string FormatLanguageLabel(string languageBucket)
        => languageBucket == InvariantLanguageBucket
            ? InvariantLanguageBucketLabel
            : languageBucket;

    /// <summary>
    /// Describes where the conflicting value currently lives for user-facing messages.
    /// </summary>
    private static string DescribeConflictTarget(UniqueValueEntry conflict)
        => conflict.Source == UniqueValueSource.Pending
            ? $"another item in the same request (item {conflict.Index})"
            : $"saved entity {conflict.Entity.EntityId}";

    /// <summary>
    /// Describes an entity involved in a unique conflict for diagnostics logging.
    /// </summary>
    private static string DescribeEntity(UniqueValueEntry entry)
    {
        var entity = entry.Entity;
        return $"source:{entry.Source}, item:{entry.Index}, entityId:{entity.EntityId}, repoId:{entity.RepositoryId}, guid:{entity.EntityGuid}, logicalId:{FormatLogicalId(entry.LogicalId)}";
    }

    /// <summary>
    /// Converts a <see cref="LogicalId"/> into the short diagnostic form used in log messages.
    /// </summary>
    private static string FormatLogicalId(LogicalId logicalId)
        => logicalId.Kind switch
        {
            LogicalIdKind.Guid => "g:" + logicalId.Guid,
            LogicalIdKind.PendingIndex => "p:" + logicalId.Primary,
            LogicalIdKind.RepositoryId => "r:" + logicalId.Primary,
            LogicalIdKind.EntityIdAndIndex => $"e:{logicalId.Primary}:{logicalId.Secondary}",
            _ => string.Empty
        };

    private readonly record struct UniqueValueKey(
        string ContentTypeNameId,
        string FieldName,
        string LanguageBucket,
        string NormalizedKey
    );

    private readonly record struct UniqueValueEntry(
        UniqueValueKey Key,
        string ContentTypeNameId,
        string FieldName,
        string DisplayValue,
        string LanguageBucket,
        UniqueValueSource Source,
        LogicalId LogicalId,
        IEntity Entity,
        int Index
    );

    private readonly record struct NormalizedValue(string NormalizedKey, string DisplayValue);

    /// <summary>
    /// Compact value object describing the logical entity identity behind a uniqueness entry.
    /// </summary>
    /// <remarks>
    /// Equality on this struct answers the question: "should these entries be treated as the same item
    /// even if they appear more than once during validation?"
    /// </remarks>
    private readonly record struct LogicalId(LogicalIdKind Kind, Guid Guid, int Primary, int Secondary);

    private sealed class UniqueValueKeyComparer : IEqualityComparer<UniqueValueKey>
    {
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

        public bool Equals(UniqueValueKey x, UniqueValueKey y)
            => Comparer.Equals(x.ContentTypeNameId, y.ContentTypeNameId)
               && Comparer.Equals(x.FieldName, y.FieldName)
               && Comparer.Equals(x.LanguageBucket, y.LanguageBucket)
               && Comparer.Equals(x.NormalizedKey, y.NormalizedKey);

        public int GetHashCode(UniqueValueKey obj)
        {
            var hash = new HashCode();
            hash.Add(obj.ContentTypeNameId, Comparer);
            hash.Add(obj.FieldName, Comparer);
            hash.Add(obj.LanguageBucket, Comparer);
            hash.Add(obj.NormalizedKey, Comparer);
            return hash.ToHashCode();
        }
    }

    /// <summary>
    /// Identifies whether a uniqueness entry came from persisted data or from the current save request.
    /// </summary>
    /// <remarks>
    /// This source influences both conflict wording and the fallback strategy used by <see cref="BuildLogicalId"/>.
    /// </remarks>
    private enum UniqueValueSource
    {
        Existing,
        Pending
    }

    /// <summary>
    /// Describes which fields on <see cref="LogicalId"/> contain the meaningful identity payload.
    /// </summary>
    private enum LogicalIdKind : byte
    {
        /// <summary>
        /// The logical identity is the stable entity guid stored in <see cref="LogicalId.Guid"/>.
        /// This is the preferred form because it survives persisted and pending representations.
        /// </summary>
        Guid,

        /// <summary>
        /// The logical identity is the zero-based item index of a pending request stored in <see cref="LogicalId.Primary"/>.
        /// Used for new items that do not have a guid yet, so they still do not collide with themselves inside one request.
        /// </summary>
        PendingIndex,

        /// <summary>
        /// The logical identity is the persisted repository id stored in <see cref="LogicalId.Primary"/>.
        /// This is a fallback for existing items when no guid is available but repository identity is known.
        /// </summary>
        RepositoryId,

        /// <summary>
        /// The logical identity combines a fallback entity id in <see cref="LogicalId.Primary"/> with the enumeration index in <see cref="LogicalId.Secondary"/>.
        /// This last-resort form keeps legacy or partially populated entities distinct even when no better stable identifier exists.
        /// </summary>
        EntityIdAndIndex
    }
}
