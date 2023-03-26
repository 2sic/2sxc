using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Linking;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IEntity = ToSic.Eav.Data.IEntity;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCode
    {


        #region AsDynamic and AsEntity

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => _DynCodeRoot?.AsDynamic(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot?.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot?.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot?.AsDynamic(entities);

        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot?.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list)
            => _DynCodeRoot?.AsList(list);


        #endregion


        #region CreateSource
        /// <inheritdoc />
        public T CreateSource<T>(IDataStream source) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(source);

        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, object options = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, options);


        #endregion

        [PrivateApi]
        public IDataSource CreateDataSource(string noParamOrder = Protector, string name = default, IDataSourceLinkable attach = default, object options = default)
            => _DynCodeRoot.CreateDataSource(noParamOrder: name, attach: attach, options: options);

        public T CreateDataSource<T>(string noParamOrder = Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource 
            => _DynCodeRoot.CreateDataSource<T>(noParamOrder: noParamOrder, attach: attach, options: options);


        #region AsAdam
        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
            => _DynCodeRoot?.AsAdam(entity, fieldName);

        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName)
            => _DynCodeRoot?.AsAdam(entity, fieldName);
        #endregion
    }
}
