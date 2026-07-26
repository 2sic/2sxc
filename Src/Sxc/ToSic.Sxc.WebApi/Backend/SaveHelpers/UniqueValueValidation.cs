using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Services;

namespace ToSic.Sxc.Backend.SaveHelpers;

[PrivateApi]
[VisualQuery(
    NiceName = "Unique Value Validation",
    NameId = "f5a8f940-8db6-4cda-861c-62e4105ea562",
    NameIds = ["System.UniqueValueValidation"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Internal,
    UiHint = "Validate one field value for uniqueness"
)]
public class UniqueValueValidation : CustomDataSource
{
    [Configuration(Fallback = "[QueryString:ContentTypeName]")]
    public string? ContentTypeName => Configuration.GetThis();

    [Configuration(Fallback = "[QueryString:FieldName]")]
    public string? FieldName => Configuration.GetThis();

    [Configuration(Fallback = "[QueryString:Value]")]
    public string? Value => Configuration.GetThis();

    [Configuration(Fallback = "[QueryString:Language]")]
    public string? Language => Configuration.GetThis();

    [Configuration(Fallback = "[QueryString:CurrentEntityGuid]")]
    public string? CurrentEntityGuid => Configuration.GetThis();

    [Configuration(Fallback = "[QueryString:CurrentEntityId]")]
    public string? CurrentEntityId => Configuration.GetThis();

    public UniqueValueValidation(
        Dependencies services,
        IAppReaderFactory appReaders,
        IDataSourcesService dataSources)
        : base(services, logName: "Sxc.UnqVal", connect: [appReaders, dataSources])
    {
        _appReaders = appReaders;
        _dataSources = dataSources;
        _lookup = new(dataSources, Log);

        ProvideOutRaw(GetValidationResult, options: () => new()
        {
            AutoId = false,
            TitleField = nameof(ValidationResultRaw.Reason),
            TypeName = "UniqueValueValidation",
        });
    }

    private readonly IAppReaderFactory _appReaders;
    private readonly IDataSourcesService _dataSources;
    private readonly UniqueValueLookup _lookup;

    private IEnumerable<IRawEntity> GetValidationResult()
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>($"{ContentTypeName}.{FieldName}");

        var appReader = _appReaders.Get(this);
        var currentEntity = ResolveCurrentEntity(appReader);
        var appData = _dataSources.CreateDefault(new DataSourceOptions
        {
            AppIdentityOrReader = this,
            LookUp = Configuration.LookUpEngine,
        });
        var result = Validate(
            appReader,
            appData,
            _lookup,
            new(
                ContentTypeName,
                FieldName,
                Value,
                Language,
                currentEntity
            )
        );

        return l.Return([Result(result)], result.IsValid ? "unique" : $"conflict:{result.ConflictEntityId}");
    }

    private IEntity? ResolveCurrentEntity(IAppReader appReader)
    {
        if (Guid.TryParse(CurrentEntityGuid, out var entityGuid))
            return appReader.List.GetOne(entityGuid);

        return int.TryParse(CurrentEntityId, out var entityId)
            ? appReader.List.GetOne(entityId)
            : null;
    }

    internal static UniqueValueValidationResult Validate(IAppReadContentTypes appReader, IDataSource appData, UniqueValueLookup lookup, UniqueValueValidationRequest request)
    {
        var contentType = ResolveContentType(appReader, request.ContentTypeName);
        if (contentType == null)
            return new(true, "type-not-found");

        var field = ResolveField(contentType, request.FieldName);
        if (field == null)
            return new(true, "field-not-found", ContentTypeName: contentType.NameId);

        if (!UniqueValueValidationRules.IsUniqueField(field))
            return new(true, "not-applicable", ContentTypeName: contentType.NameId, FieldName: field.Name);

        var normalizedValue = UniqueValueValidationRules.NormalizedValue(field.Type, request.Value);
        if (normalizedValue == null)
            return new(true, "blank", ContentTypeName: contentType.NameId, FieldName: field.Name);

        var language = request.Language ?? UniqueValueValidationRules.InvariantLanguage;
        var conflict = lookup.FindConflict(
            appData,
            new(
                contentType.NameId,
                field.Name,
                field.Type,
                normalizedValue,
                request.CurrentEntity,
                UniqueValueValidationRules.LanguageFilterOrNull(language)
            )
        );

        return conflict == null
            ? new(true, "ok", contentType.NameId, field.Name, normalizedValue, request.Language)
            : new(false, "duplicate", contentType.NameId, field.Name, normalizedValue, request.Language, conflict.EntityId, conflict.EntityGuid, conflict.GetBestTitle());
    }

    private static IContentType? ResolveContentType(IAppReadContentTypes appReader, string? contentTypeName)
        => string.IsNullOrWhiteSpace(contentTypeName)
            ? null
            : appReader.TryGetContentType(contentTypeName);

    private static IContentTypeField? ResolveField(IContentType contentType, string? fieldName)
        => string.IsNullOrWhiteSpace(fieldName)
            ? null
            : contentType.Attributes.FirstOrDefault(attribute => attribute.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

    private IRawEntity Result(UniqueValueValidationResult result)
        => new RawEntity
        {
            Id = result.ConflictEntityId ?? 0,
            Guid = result.ConflictGuid ?? Guid.Empty,
            Values = new Dictionary<string, object?>
            {
                { nameof(ValidationResultRaw.IsValid), result.IsValid },
                { nameof(ValidationResultRaw.Reason), result.Reason },
                { nameof(ValidationResultRaw.ContentTypeName), result.ContentTypeName ?? ContentTypeName },
                { nameof(ValidationResultRaw.FieldName), result.FieldName ?? FieldName },
                { nameof(ValidationResultRaw.Value), result.Value ?? Value },
                { nameof(ValidationResultRaw.Language), result.Language ?? Language },
                { nameof(ValidationResultRaw.ConflictEntityId), result.ConflictEntityId },
                { nameof(ValidationResultRaw.ConflictGuid), result.ConflictGuid },
                { nameof(ValidationResultRaw.ConflictTitle), result.ConflictTitle },
            },
        };

    private sealed class ValidationResultRaw
    {
        public bool IsValid { get; init; }
        public string? Reason { get; init; }
        public string? ContentTypeName { get; init; }
        public string? FieldName { get; init; }
        public string? Value { get; init; }
        public string? Language { get; init; }
        public int? ConflictEntityId { get; init; }
        public Guid? ConflictGuid { get; init; }
        public string? ConflictTitle { get; init; }
    }
}

internal sealed record UniqueValueValidationRequest(
    string? ContentTypeName,
    string? FieldName,
    string? Value,
    string? Language = default,
    IEntity? CurrentEntity = default
);

internal sealed record UniqueValueValidationResult(
    bool IsValid,
    string Reason,
    string? ContentTypeName = default,
    string? FieldName = default,
    string? Value = default,
    string? Language = default,
    int? ConflictEntityId = default,
    Guid? ConflictGuid = default,
    string? ConflictTitle = default
);