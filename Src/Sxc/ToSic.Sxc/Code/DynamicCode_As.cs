using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCode
    {


        #region AsDynamic and AsEntity

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => UnwrappedContents?.AsDynamic(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => UnwrappedContents?.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) => UnwrappedContents?.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) => UnwrappedContents?.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(dynamic list)
            => UnwrappedContents?.AsList(list);


        #endregion

        #region CreateSource
        /// <inheritdoc />
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => UnwrappedContents.CreateSource<T>(inStream);

        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => UnwrappedContents.CreateSource<T>(inSource, configurationProvider);

        #endregion

        #region AsAdam
        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
            => UnwrappedContents?.AsAdam(entity, fieldName);

        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName)
            => UnwrappedContents?.AsAdam(entity, fieldName);
        #endregion
    }
}
