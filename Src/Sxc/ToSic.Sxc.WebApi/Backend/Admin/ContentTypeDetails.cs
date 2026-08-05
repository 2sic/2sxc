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
            AllowUnknownValueTypes = true,
        });

        ProvideOutRaw(GetFields, name: "Fields", options: () => new()
        {
            AllowUnknownValueTypes = true,
        });
    }

    private IEnumerable<ContentTypeDto> GetContentTypeDetails()
    {
        var l = Log.Fn<IEnumerable<ContentTypeDto>>();

        var appCtxPlus = _workEntities.CtxSvc.ContextPlus(AppId);
        var contentType = appCtxPlus.AppReader.TryGetContentType(ContentTypeId);

        if (contentType == null)
            return l.Return([], "not found");

        var dto = _convTypeDto.Convert(contentType);

        return l.Return([dto], "ok");
    }

    private IEnumerable<ContentTypeFieldDto> GetFields()
    {
        var l = Log.Fn<IEnumerable<ContentTypeFieldDto>>();

        var fields = _workAttributes.New(AppId).GetFields(ContentTypeId);

        var convertedFields = _convAttrDto.New()
            .Init(AppId, false)
            .Convert(fields)
            .ToListOpt();

        return l.Return(convertedFields, $"{convertedFields.Count}");
    }

}
