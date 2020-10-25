using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc.RazorPages.Exp
{
    public partial class PageBaseLoadingBlogOnly: IDynamicCode
    {

        #region Content, Header, etc. and List
        /// <inheritdoc/>
        public new dynamic Content => DynCode.Content;

        /// <inheritdoc />
        public dynamic Header => DynCode.Header;

        #endregion


        #region Link, Edit, App, Data

        /// <inheritdoc />
        public ILinkHelper Link => DynCode.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => DynCode.Edit;

        [PrivateApi] public int CompatibilityLevel => DynCode.CompatibilityLevel;

        /// <inheritdoc />
        public IApp App => DynCode.App;

        /// <inheritdoc />
        public IBlockDataSource Data => DynCode.Data;

        #endregion

        #region AsDynamic in many variations

        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => DynCode.AsDynamic(json, fallback);

        /// <inheritdoc/>
        public dynamic AsDynamic(IEntity entity) => DynCode.AsDynamic(entity);

        /// <inheritdoc/>
        public dynamic AsDynamic(dynamic dynamicEntity) => DynCode.AsDynamic(dynamicEntity);


        #endregion

        #region AsEntity
        /// <inheritdoc/>
        public IEntity AsEntity(dynamic dynamicEntity) => DynCode.AsEntity(dynamicEntity);
        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(dynamic list) => DynCode?.AsList(list);

        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
        {
            throw new NotImplementedException();
        }

        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null) where T : IDataSource
        {
            throw new NotImplementedException();
        }

        #endregion



        #region Adam 

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => DynCode.AsAdam(entity, fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => DynCode.AsAdam(entity, fieldName);

        #endregion

    }
}
