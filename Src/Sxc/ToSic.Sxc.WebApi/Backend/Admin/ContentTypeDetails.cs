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

    public ContentTypeDetails(
        Dependencies services,
        GenWorkPlus<WorkEntities> workEntities,
        ConvertContentTypeToDto convTypeDto)
        : base(services, logName: "Eav.CtDetails", connect: [workEntities, convTypeDto])
    {
        _workEntities = workEntities;
        _convTypeDto = convTypeDto;

        ProvideOutRaw(GetContentTypeDetails, options: () => new()
        {
            TitleField = nameof(ContentTypeDto.Name),
            TypeName = "ContentType",
        });
    }

    private IEnumerable<IRawEntity> GetContentTypeDetails()
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();

        var appId = Configuration.GetThis(0, "AppId");
        var contentTypeStaticName = Configuration.GetThis<string>("ContentTypeId");

        var appCtxPlus = _workEntities.CtxSvc.ContextPlus(appId);
        var contentType = appCtxPlus.AppReader.GetContentType(contentTypeStaticName);

        if (contentType == null)
            return l.Return([], "not found");

        var dto = _convTypeDto.Convert(contentType);

        var entity = new RawEntity(new()
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
        });

        return l.Return([entity], "ok");
    }
}