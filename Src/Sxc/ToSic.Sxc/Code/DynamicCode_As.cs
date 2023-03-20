using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IEntity = ToSic.Eav.Data.IEntity;

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
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inStream);

        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        [PrivateApi]
        public IDataSource CreateSourceWip(string name, IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            // where T : IDataSource
            => _DynCodeRoot.CreateSourceWip(name, inSource, configurationProvider);

        #endregion

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
