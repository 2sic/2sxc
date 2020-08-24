using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Sxc.Conversion;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Compatibility.Sxc
{
    /// <summary>
    /// This is for compatibility - old code had a Sxc.Serializer.Prepare code which should still work
    /// </summary>
    public class OldDataToDictionaryWrapper
    {
        public OldDataToDictionaryWrapper(bool userMayEdit)
        {
            _converter = new DataToDictionary(userMayEdit);
        }

        private readonly DataToDictionary _converter;

        public IEnumerable<IDictionary<string, object>> Prepare(IEnumerable<dynamic> dynamicList)
            => _converter.Convert(dynamicList);

        public IDictionary<string, object> Prepare(IDynamicEntity dynamicEntity)
            => _converter.Convert(dynamicEntity);

        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Prepare(IDataSource source,
            IEnumerable<string> streams = null)
            => _converter.Convert(source, streams);

        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Prepare(IDataSource source, string streams)
            => _converter.Convert(source, streams);

        public IEnumerable<IDictionary<string, object>> Prepare(IDataStream stream)
            => _converter.Convert(stream);

        public IEnumerable<IDictionary<string, object>> Prepare(IEnumerable<IEntity> entities)
            => _converter.Convert(entities);

        public IEnumerable<IDictionary<string, object>> Prepare(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
            => _converter.Convert(entities);

        public Dictionary<string, object> Prepare(IEntity entity)
            => _converter.Convert(entity);

    }
}
