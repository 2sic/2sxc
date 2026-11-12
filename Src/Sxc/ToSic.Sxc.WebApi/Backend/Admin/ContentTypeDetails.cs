using ToSic.Eav.Data.ContentTypes.Sys;
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
[ShowApiWhenReleased(ShowApiMode.Never)]
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
        ConvertContentTypeToRaw convTypeRaw,
        AppWorkQuick<WorkAttributes> workAttributes,
        Generator<ConvertAttributeToRaw> convAttrDto)
        : base(services, logName: "Eav.CtDetails", connect: [workEntities, convTypeRaw, workAttributes, convAttrDto])
    {

        ProvideOutRaw(() => GetContentTypeDetails(workEntities.New(AppId), convTypeRaw));

        ProvideOutRaw(() => GetFields(workAttributes.New(AppId), convAttrDto), name: "Fields");
    }

    private IEnumerable<ContentTypeRaw> GetContentTypeDetails(WorkEntities workEntities,
        ConvertContentTypeToRaw convTypeRaw)
    {
        var l = Log.Fn<IEnumerable<ContentTypeRaw>>();

        var contentType = workEntities.MyOptions.AppReader.TryGetContentType(ContentTypeId);

        if (contentType == null)
            return l.Return([], "not found");

        var dto = convTypeRaw.Convert(contentType);

        return l.Return([dto], "ok");
    }

    private IEnumerable<ContentTypeFieldRaw> GetFields(WorkAttributes workAttributes,
        Generator<ConvertAttributeToRaw> convAttrDto)
    {
        var l = Log.Fn<IEnumerable<ContentTypeFieldRaw>>();

        var fields = workAttributes.GetFields(ContentTypeId);

        var convertedFields = convAttrDto.New()
            .Init(AppId, false)
            .Convert(fields)
            .ToListOpt();

        return l.Return(convertedFields, $"{convertedFields.Count}");
    }

}
