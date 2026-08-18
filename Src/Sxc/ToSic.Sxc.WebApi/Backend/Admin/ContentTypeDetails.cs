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
    #region Configuration Properties

    /// <summary>
    /// The GUID of the content type.
    /// </summary>
    [Configuration(Fallback = "")]
    public string ContentTypeId => Configuration.GetThis(fallback: "");

    #endregion

    public ContentTypeDetails(
        Dependencies services,
        AppWorkQuick<WorkEntities> workEntities,
        ConvertContentTypeToDto convTypeDto,
        AppWorkQuick<WorkAttributes> workAttributes,
        Generator<ConvertAttributeToDto> convAttrDto)
        : base(services, logName: "Eav.CtDetails", connect: [workEntities, convTypeDto, workAttributes, convAttrDto])
    {

        ProvideOutRaw(() => GetContentTypeDetails(workEntities.New(AppId), convTypeDto), options: () => new()
        {
            AllowUnknownValueTypes = true,
        });

        ProvideOutRaw(() => GetFields(workAttributes.New(AppId), convAttrDto), name: "Fields", options: () => new()
        {
            AllowUnknownValueTypes = true,
        });
    }

    private IEnumerable<ContentTypeDto> GetContentTypeDetails(WorkEntities workEntities,
        ConvertContentTypeToDto convTypeDto)
    {
        var l = Log.Fn<IEnumerable<ContentTypeDto>>();

        var contentType = workEntities.MyOptions.AppReader.TryGetContentType(ContentTypeId);

        if (contentType == null)
            return l.Return([], "not found");

        var dto = convTypeDto.Convert(contentType);

        return l.Return([dto], "ok");
    }

    private IEnumerable<ContentTypeFieldDto> GetFields(WorkAttributes workAttributes,
        Generator<ConvertAttributeToDto> convAttrDto)
    {
        var l = Log.Fn<IEnumerable<ContentTypeFieldDto>>();

        var fields = workAttributes.GetFields(ContentTypeId);

        var convertedFields = convAttrDto.New()
            .Init(AppId, false)
            .Convert(fields)
            .ToListOpt();

        return l.Return(convertedFields, $"{convertedFields.Count}");
    }

}
