using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Environment;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Search;
using ToSic.Sxc.Web;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;


namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// The base class for Razor-Components in 2sxc 10+ <br/>
    /// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
    /// </summary>
    [PublicApi]
    public abstract partial class RazorComponent : RazorComponentBase, IRazorComponent
    {

        #region Link, Edit, Dnn, App, Data

        /// <inheritdoc />
        public ILinkHelper Link => DynCode.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => DynCode.Edit;

        /// <inheritdoc />
        public IDnnContext Dnn => DynCode.Dnn;

        /// <inheritdoc />
        [PrivateApi("todo: try to remove thi")]
        public SxcHelper Sxc => DynCode.Sxc;

        /// <inheritdoc />
        public new IApp App => DynCode.App;

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

        public IEnumerable<dynamic> AsListTest(dynamic list) => AsList(list);

        #endregion


        #region Data Source Stuff

        /// <inheritdoc/>
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => DynCode.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc/>
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => DynCode.CreateSource<T>(inStream);

        #endregion



        #region Content, Header, etc. and List
        /// <inheritdoc/>
        public dynamic Content => DynCode.Content;

        /// <inheritdoc />
        public dynamic Header => DynCode.Header;

        #endregion



        #region Customize Data, Search, and Purpose

        /// <inheritdoc />
        public virtual void CustomizeData() { }

        /// <inheritdoc />
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo,
            DateTime beginDate)  { }

        /// <inheritdoc />
        public Purpose Purpose { get; internal set; }

        #endregion


        #region Adam 

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => DynCode.AsAdam(entity, fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => DynCode.AsAdam(entity, fieldName);

        #endregion

    }
}
