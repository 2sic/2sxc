using System.Net.Mime;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace ToSic.Eav.WebApi.Sys.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "Content Type Details",
    NameId = "c56b9706-c06a-4d48-a362-d0e0036733d4",
    NameIds = ["System.ContentTypeDetails"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Internal,
    UiHint = "Details of a single content type"
)]
public class ContentTypeDetails : CustomDataSource
{
    private readonly GenWorkPlus<WorkEntities> _workEntities;
    private readonly ConvertContentTypeToDto _convTypeDto;
    private readonly GenWorkBasic<WorkAttributes> _workAttributes;
    private readonly Generator<ConvertAttributeToDto> _convAttrDto;

    #region Configuration Properties

    /// <summary>
    /// The GUID of the content type.
    /// </summary>
    [Configuration(Fallback = "")]
    public string ContentTypeId => Configuration.GetThis(fallback: "");

    #endregion

    public ContentTypeDetails(
        Dependencies services,
        GenWorkPlus<WorkEntities> workEntities,
        ConvertContentTypeToDto convTypeDto,
        GenWorkBasic<WorkAttributes> workAttributes,
        Generator<ConvertAttributeToDto> convAttrDto)
        : base(services, logName: "Eav.CtDetails", connect: [workEntities, convTypeDto, workAttributes, convAttrDto])
    {
        _workEntities = workEntities;
        _convTypeDto = convTypeDto;
        _workAttributes = workAttributes;
        _convAttrDto = convAttrDto;

        ProvideOutRaw(GetContentTypeDetails, options: () => new()
        {
            TitleField = nameof(ContentTypeDto.Name),
            TypeName = IContentTypeDetails.Constants.ContentTypeName,
            AllowUnknownValueTypes = true,
        });

        ProvideOutRaw(GetFields, name: "Fields", options: () => new()
        {
            TitleField = nameof(ContentTypeFieldDto.StaticName),
            TypeName = "ContentTypeField",
            AllowUnknownValueTypes = true,
        });
    }

    private IEnumerable<IRawEntity> GetContentTypeDetails()
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();

        var appCtxPlus = _workEntities.CtxSvc.ContextPlus(AppId);
        var contentType = appCtxPlus.AppReader.GetContentType(ContentTypeId);

        if (contentType == null)
            return l.Return([], "not found");

        var dto = _convTypeDto.Convert(contentType);

        var entity = new RawEntity
        {
            Id = dto.Id,
            Values = new Dictionary<string, object?>
            {
                { nameof(ContentTypeDto.Id), dto.Id },
                { nameof(ContentTypeDto.Name), dto.Name },
                { nameof(ContentTypeDto.Label), dto.Label },
                { nameof(ContentTypeDto.StaticName), dto.StaticName },
                { nameof(ContentTypeDto.NameId), dto.NameId },
                { nameof(ContentTypeDto.Scope), dto.Scope },
                { nameof(ContentTypeDto.Description), dto.Description },
                { nameof(ContentTypeDto.UsesSharedDef), dto.UsesSharedDef },
                { nameof(ContentTypeDto.SharedDefId), dto.SharedDefId },
                { nameof(ContentTypeDto.Items), dto.Items },
                { nameof(ContentTypeDto.Fields), dto.Fields },
                { nameof(ContentTypeDto.TitleField), dto.TitleField },
                { nameof(ContentTypeDto.Metadata), dto.Metadata },
                { nameof(ContentTypeDto.Properties), dto.Properties },
                { nameof(ContentTypeDto.Permissions), dto.Permissions },
                { nameof(ContentTypeDto.EditInfo), dto.EditInfo },
            },
        };

        return l.Return([entity], "ok");
    }

    private IEnumerable<IRawEntity> GetFields()
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();

        var fields = _workAttributes.New(AppId).GetFields(ContentTypeId);

        var convertedFields = _convAttrDto.New()
            .Init(AppId, false)
            .Convert(fields);

        var entities = convertedFields
            .Select(field => new RawEntity
            {
                Id = field.Id,
                Values = new Dictionary<string, object?>
                {
                    { nameof(ContentTypeFieldDto.Id), field.Id },
                    { nameof(ContentTypeFieldDto.SortOrder), field.SortOrder },
                    { nameof(ContentTypeFieldDto.Type), field.Type },
                    { nameof(ContentTypeFieldDto.InputType), field.InputType },
                    { nameof(ContentTypeFieldDto.StaticName), field.StaticName },
                    { nameof(ContentTypeFieldDto.IsTitle), field.IsTitle },
                    { nameof(ContentTypeFieldDto.AttributeId), field.AttributeId },
                    { nameof(ContentTypeFieldDto.Metadata), field.Metadata },
                    { nameof(ContentTypeFieldDto.InputTypeConfig), field.InputTypeConfig },
                    { nameof(ContentTypeFieldDto.Permissions), field.Permissions },
                    { nameof(ContentTypeFieldDto.ImageConfiguration), field.ImageConfiguration },
                    { nameof(ContentTypeFieldDto.IsEphemeral), field.IsEphemeral },
                    { nameof(ContentTypeFieldDto.HasFormulas), field.HasFormulas },
                    { nameof(ContentTypeFieldDto.EditInfo), field.EditInfo },
                    { nameof(ContentTypeFieldDto.Guid), field.Guid },
                    { nameof(ContentTypeFieldDto.SysSettings), field.SysSettings },
                    { nameof(ContentTypeFieldDto.ContentType), field.ContentType },
                    { nameof(ContentTypeFieldDto.ConfigTypes), field.ConfigTypes },
                },
            })
            .ToList();

        return l.Return(entities, $"{entities.Count}");
    }

}