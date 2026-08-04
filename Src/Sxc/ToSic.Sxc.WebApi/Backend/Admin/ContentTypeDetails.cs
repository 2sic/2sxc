using ToSic.Eav.Data.ContentTypes;
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

    private IEnumerable<ContentTypeDetailsModel> GetContentTypeDetails()
    {
        var l = Log.Fn<IEnumerable<ContentTypeDetailsModel>>();

        var appCtxPlus = _workEntities.CtxSvc.ContextPlus(AppId);
        var contentType = appCtxPlus.AppReader.GetContentType(ContentTypeId);

        if (contentType == null)
            return l.Return([], "not found");

        var dto = _convTypeDto.Convert(contentType);

        var entity = new ContentTypeDetailsModel(dto)
        {
            Id = dto.Id,
        };

        return l.Return([entity], "ok");
    }

    private IEnumerable<ContentTypeFieldModel> GetFields()
    {
        var l = Log.Fn<IEnumerable<ContentTypeFieldModel>>();

        var fields = _workAttributes.New(AppId).GetFields(ContentTypeId);

        var convertedFields = _convAttrDto.New()
            .Init(AppId, false)
            .Convert(fields);

        var entities = convertedFields.Select(field => new ContentTypeFieldModel(field)
        {
            Id = field.Id,
        }).ToList();

        return l.Return(entities, $"{entities.Count}");
    }

}
