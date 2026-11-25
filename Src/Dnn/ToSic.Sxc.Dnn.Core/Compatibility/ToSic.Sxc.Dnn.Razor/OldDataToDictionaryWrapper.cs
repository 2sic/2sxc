using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource;

namespace ToSic.Sxc.Dnn.Razor;

/// <summary>
/// This is for compatibility - old code had a Sxc.Serializer.Prepare code which should still work
/// </summary>
[Obsolete]
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class OldDataToDictionaryWrapper
{
    public OldDataToDictionaryWrapper(bool userMayEdit, IConvertToEavLight innerConverter)
    {
        _converter = innerConverter;
        // 2026-11-11 2dm - dropped this, believe it's not used anymore; keep for reference till ca. 2027-07 #DropWithEdit
        //if (_converter is ConvertToEavLightWithCmsInfo serializerWithEdit) serializerWithEdit.WithEdit = userMayEdit;
    }

    private readonly IConvertToEavLight _converter;

    public IEnumerable<IDictionary<string, object>> Prepare(IEnumerable<dynamic> dynamicList)
        => _converter.Convert(dynamicList);

    public IDictionary<string, object> Prepare(ICanBeEntity dynamicEntity)
        => _converter.Convert(dynamicEntity);

    public IDictionary<string, IEnumerable<EavLightEntity>> Prepare(IDataSource source,
        IEnumerable<string> streams = null)
        => _converter.Convert(source, streams);

    public IDictionary<string, IEnumerable<EavLightEntity>> Prepare(IDataSource source, string streams)
        => _converter.Convert(source, streams);

    public IEnumerable<IDictionary<string, object>> Prepare(IDataStream stream)
        => _converter.Convert(stream);

    public IEnumerable<IDictionary<string, object>> Prepare(IEnumerable<IEntity> entities)
        => _converter.Convert(entities);

    public IDictionary<string, object> Prepare(IEntity entity)
        => _converter.Convert(entity);

}