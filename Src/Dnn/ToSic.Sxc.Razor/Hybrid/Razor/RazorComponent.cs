using System;
using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Run.Context;
using ToSic.Sxc.Search;
using ToSic.Sxc.Web;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;


namespace ToSic.Sxc.Hybrid.Razor
{
    /// <summary>
    /// The base class for Razor-Components in 2sxc 10+ <br/>
    /// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract partial class RazorComponent : RazorComponentBase //, IRazorComponent
    {

        #region Link, Edit, Dnn, App, Data

        /// <inheritdoc />
        public ILinkHelper Link => DynCode.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => DynCode.Edit;

        ///// <inheritdoc />
        //public IDnnContext Dnn => DynCode.Dnn;

        [PrivateApi] public IBlock Block => throw new NotSupportedException("don't use this");

        [PrivateApi] public IServiceProvider ServiceProvider => DynCode.ServiceProvider;

        [PrivateApi] public int CompatibilityLevel => DynCode.CompatibilityLevel;

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
        public virtual void CustomizeData()
        {
            // new in 2sxc 11, if it has not been overridden, then try to check if code has something for us.
            var code = CodeManager.CodeOrNull;
            if (code == null) return;
            if (code is RazorComponent codeAsRazor) codeAsRazor.CustomizeData();
        }

        /// <inheritdoc />
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo,
            DateTime beginDate)
        {
            // new in 2sxc 11, if it has not been overridden, then try to check if code has something for us.
            var code = CodeManager.CodeOrNull;
            if (code == null) return;
            if (code is RazorComponent codeAsRazor) codeAsRazor.CustomizeSearch(searchInfos, moduleInfo, beginDate);
        }

        /// <inheritdoc />
        public Purpose Purpose { get; internal set; }

        #endregion


        #region Adam 

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => DynCode.AsAdam(entity, fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => DynCode.AsAdam(entity, fieldName);

        #endregion

        #region RunContext WIP

        [PrivateApi] public RunContext RunContext => DynCode?.RunContext;

        #endregion
    }
}
