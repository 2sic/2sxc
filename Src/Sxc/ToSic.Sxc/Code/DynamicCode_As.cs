using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCode
    {


        #region AsDynamic and AsEntity

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot?.AsC.AsDynamicFromJson(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot?.AsC.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot?.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot?.AsC.MergeDynamic(entities);

        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot?.AsC.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list)
            => _DynCodeRoot?.AsC.AsDynamicList(list);


        #endregion


        #region CreateSource
        /// <inheritdoc />
        public T CreateSource<T>(IDataStream source) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(source);

        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);


        #endregion

        #region AsAdam
        /// <inheritdoc />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot?.AsAdam(item, fieldName);

        #endregion
    }
}
