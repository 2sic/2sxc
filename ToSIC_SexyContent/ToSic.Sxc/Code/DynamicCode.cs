using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
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
    public abstract class DynamicCode : IDynamicCode, IWrapper<IDynamicCode>, ICoupledDynamicCode, IHasLog
    {
        [PrivateApi] public int CompatibilityLevel => UnwrappedContents?.CompatibilityLevel ?? 9;

        /// <inheritdoc />
        public IApp App => UnwrappedContents?.App;

        /// <inheritdoc />
        public IBlockDataSource Data => UnwrappedContents?.BlockBuilder?.Block?.Data;

        [PrivateApi] public IBlockBuilder BlockBuilder => UnwrappedContents?.BlockBuilder;

        /// <inheritdoc />
        /// <remarks>
        /// The parent of this object. It's not called Parent but uses an exotic name to ensure that your code won't accidentally create a property with the same name.
        /// </remarks>
        public IDynamicCode UnwrappedContents { get; private set; }

        [PrivateApi]
        public virtual void DynamicCodeCoupling(IDynamicCode parent, string path)
        {
            UnwrappedContents = parent;
            try { 
                Log.Add("DynamicCode created: " + path);
            } catch { /* ignore */ }
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

        public ILog Log => UnwrappedContents?.Log;
    }
}
