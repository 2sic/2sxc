using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Eav.Services;

namespace ToSic.Sxc.Backend.SaveHelpers;

/// <summary>
/// Reusable exact-value uniqueness lookup for one content-type field.
/// </summary>
internal sealed class UniqueValueLookup(IDataSourcesService dataSources, ILog parentLog) : HelperBase(parentLog, "Val.UnqLkp")
{
    private const string MaxCandidatesToInspect = "3";

    internal IEntity? FindConflict(IDataSource appData, UniqueValueLookupRequest request)
    {
        var l = Log.Fn<IEntity?>($"{request.ContentTypeNameId}.{request.FieldName}", timer: true);

        if (!IsSupported(request.FieldType) || string.IsNullOrWhiteSpace(request.Value))
            return l.ReturnNull("ignored");

        // The datasource may still return the current entity, so the final pass must exclude it
        // before treating the result as a uniqueness conflict.
        var current = request.CurrentEntity;
        var conflict = MatchingCandidates(appData, request)
            .FirstOrDefault(entity => current == null || !IsSameEntity(entity, current));

        return l.Return(conflict, conflict == null ? "unique" : $"conflict:{conflict.EntityId}");
    }

    private IEnumerable<IEntity> MatchingCandidates(IDataSource appData, UniqueValueLookupRequest request)
    {
        // Keep the query in two steps: first narrow to the content-type, then filter by field value.
        var typeFilter = CreateTypeFilter(appData, request.ContentTypeNameId);
        var valueFilter = CreateValueFilter(typeFilter, request);

        return valueFilter.List;
    }

    private EntityTypeFilter CreateTypeFilter(IDataSource appData, string contentTypeNameId)
    {
        var typeFilter = dataSources.Create<EntityTypeFilter>(DataSourceOptions.OfDataSource(appData));
        typeFilter.TypeName = contentTypeNameId;
        return typeFilter;
    }

    private ValueFilter CreateValueFilter(IDataSource appData, UniqueValueLookupRequest request)
    {
        var valueFilter = dataSources.Create<ValueFilter>(DataSourceOptions.OfDataSource(appData));
        valueFilter.Attribute = request.FieldName;
        valueFilter.Operator = "==";
        valueFilter.Value = request.Value;
        // We only need proof of a duplicate, not a full result set.
        valueFilter.Take = MaxCandidatesToInspect;
        if (!string.IsNullOrWhiteSpace(request.Languages))
            valueFilter.Languages = request.Languages!;

        return valueFilter;
    }

    internal static bool IsSameEntity(IEntity first, IEntity second)
        => first.EntityGuid != Guid.Empty && first.EntityGuid == second.EntityGuid
           || first.RepositoryId > 0 && first.RepositoryId == second.RepositoryId
           || first.EntityId > 0 && first.EntityId == second.EntityId;

    internal static bool IsSupported(ValueTypes type)
        // Keep this in sync with IsUniqueValidator.NormalizedValue so only values that can be
        // compared reliably are sent through the lookup pipeline.
        => type is ValueTypes.String
            or ValueTypes.Hyperlink
            or ValueTypes.Custom
            or ValueTypes.Number
            // or ValueTypes.Boolean
            or ValueTypes.DateTime;
}

internal sealed record UniqueValueLookupRequest(
    string ContentTypeNameId,
    string FieldName,
    ValueTypes FieldType,
    string Value,
    IEntity? CurrentEntity = default,
    string? Languages = default
);
