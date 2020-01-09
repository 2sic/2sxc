using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// This is a base class for dynamic code which is compiled at runtime. <br/>
    /// It delegates all properties like App and methods like AsDynamic() to the parent item which initially caused it to be compiled.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class DynamicCode : IDynamicCode, IWrapper<IDynamicCode>
    {
        [PrivateApi] public int CompatibilityLevel => UnwrappedContents?.CompatibilityLevel ?? 9;

        /// <inheritdoc />
        public IApp App => UnwrappedContents?.App;

        /// <inheritdoc />
        public IBlockDataSource Data => UnwrappedContents?.CmsBlock?.Block?.Data; // Sxc?.Cms?.Block.Data;
        //[PrivateApi]
        //public SxcHelper Sxc { get; private set; }

        [PrivateApi] public ICmsBlock CmsBlock => UnwrappedContents?.CmsBlock;

        /// <inheritdoc />
        /// <remarks>
        /// The parent of this object. It's not called Parent but uses an exotic name to ensure that your code won't accidentally create a property with the same name.
        /// </remarks>
        public IDynamicCode UnwrappedContents { get; private set; }

        [PrivateApi]
        internal virtual void InitShared(IDynamicCode parent)
        {
            UnwrappedContents = parent;
            //Sxc = parent.Sxc;
        }

        #region Content and Header

        /// <inheritdoc />
        public dynamic Content => UnwrappedContents?.Content;
        /// <inheritdoc />
        public dynamic Header => UnwrappedContents?.Header;

        #endregion


        #region AsDynamic and AsEntity

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => UnwrappedContents?.AsDynamic(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => UnwrappedContents?.AsDynamic(entity);

        //[PrivateApi]
        //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        //public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => Parent?.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) => UnwrappedContents?.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) => UnwrappedContents?.AsEntity(dynamicEntity);

        //[PrivateApi]
        //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        //public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => Parent?.AsDynamic(entities);


        #endregion

        #region AsList (experimental

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(dynamic list)
            => UnwrappedContents?.AsList(list);


        #endregion

        #region CreateSource
        /// <inheritdoc />
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => UnwrappedContents.CreateSource<T>(inStream);

        //[Obsolete("use CreateSource<T> instead")]
        //public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
        //    ILookUpEngine lookUpEngine = null)
        //    => Parent?.CreateSource(typeName, inSource, lookUpEngine);

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

        #region Link and Edit
        /// <inheritdoc />
        public ILinkHelper Link => UnwrappedContents?.Link;
        /// <inheritdoc />
        public IInPageEditingSystem Edit => UnwrappedContents?.Edit;
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
            UnwrappedContents?.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, CreateInstancePath, throwOnError);

        #endregion
    }
}
