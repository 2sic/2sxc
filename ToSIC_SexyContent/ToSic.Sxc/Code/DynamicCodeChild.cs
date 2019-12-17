using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// This is a base class for dynamic code which is compiled at runtime. <br/>
    /// It delegates all properties like App and methods like AsDynamic() to the parent item. 
    /// </summary>
    public abstract class DynamicCodeChild : IDynamicCode
    {
        /// <inheritdoc />
        public IApp App => Parent?.App;

        /// <inheritdoc />
        public IBlockDataSource Data => Sxc?.Cms?.Block.Data;
        [PrivateApi]
        public SxcHelper Sxc { get; private set; }

        [PrivateApi]
        internal IDynamicCode Parent;

        [PrivateApi]
        internal virtual void InitShared(IDynamicCode parent)
        {
            Parent = parent;
            Sxc = parent.Sxc;
        }

        #region Content and Header

        /// <inheritdoc />
        public dynamic Content => Parent?.Content;
        /// <inheritdoc />
        public dynamic Header => Parent?.Header;

        #endregion


        #region AsDynamic and AsEntity

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => Parent?.AsDynamic(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => Parent?.AsDynamic(entity);

        //[PrivateApi]
        //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        //public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => Parent?.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) => Parent?.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) => Parent?.AsEntity(dynamicEntity);

        //[PrivateApi]
        //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        //public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => Parent?.AsDynamic(entities);


        #endregion

        #region AsList (experimental

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(dynamic list)
            => Parent?.AsList(list);


        #endregion

        #region CreateSource
        /// <inheritdoc />
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => Parent.CreateSource<T>(inStream);

        //[Obsolete("use CreateSource<T> instead")]
        //public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
        //    ILookUpEngine lookUpEngine = null)
        //    => Parent?.CreateSource(typeName, inSource, lookUpEngine);

        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => Parent.CreateSource<T>(inSource, configurationProvider);

        #endregion

        #region AsAdam
        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
            => Parent?.AsAdam(entity, fieldName);

        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName)
            => Parent?.AsAdam(entity, fieldName);
        #endregion

        #region Link and Edit
        /// <inheritdoc />
        public ILinkHelper Link => Parent?.Link;
        /// <inheritdoc />
        public IInPageEditingSystem Edit => Parent?.Edit;
        #endregion

        #region SharedCode - must also map previous path to use here
        /// <inheritdoc />
        public string CreateInstancePath { get; set; }

        /// <inheritdoc />
        public dynamic CreateInstance(string virtualPath, 
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            Parent?.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, CreateInstancePath, throwOnError);

        #endregion
    }
}
