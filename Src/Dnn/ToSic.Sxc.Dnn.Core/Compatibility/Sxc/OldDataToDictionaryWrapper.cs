using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Data.Internal.Convert;

namespace ToSic.Sxc.Compatibility.Sxc;

/// <summary>
/// This is for compatibility - old code had a Sxc.Serializer.Prepare code which should still work
/// </summary>
// Important: Changed Dictionary... to IDictionary in 12.04 2021-08-29 - may cause issues, but probably shouldn't
[Obsolete]
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class OldDataToDictionaryWrapper
{
    public OldDataToDictionaryWrapper(bool userMayEdit, IConvertToEavLight innerConverter)
    {
        _converter = innerConverter;
        if (_converter is ConvertToEavLightWithCmsInfo serializerWithEdit) serializerWithEdit.WithEdit = userMayEdit;
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

    public IEnumerable<IDictionary<string, object>> Prepare(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
        => _converter.Convert(entities as IEnumerable<IEntity>);

    public IDictionary<string, object> Prepare(IEntity entity)
        => _converter.Convert(entity);

}