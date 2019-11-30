using System;
using System.Collections.Generic;
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

        #region AppAndDataHelpers implementation

        /// <inheritdoc />
        public ILinkHelper Link => DynCodeHelper.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => DynCodeHelper.Edit;

        /// <inheritdoc />
        public IDnnContext Dnn => DynCodeHelper.Dnn;

        /// <inheritdoc />
        [PrivateApi("todo: try to remove thi")]
        public SxcHelper Sxc => DynCodeHelper.Sxc;

        /// <inheritdoc />
        public new IApp App => DynCodeHelper.App;

        /// <inheritdoc />
        public IBlockDataSource Data => DynCodeHelper.Data;

        #endregion

        #region AsDynamic in many variations
        /// <inheritdoc/>
        public dynamic AsDynamic(IEntity entity) => DynCodeHelper.AsDynamic(entity);

        /// <inheritdoc/>
        public dynamic AsDynamic(dynamic dynamicEntity) => DynCodeHelper.AsDynamic(dynamicEntity);

        /// <inheritdoc/>
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => DynCodeHelper.AsDynamic(stream.List);

        /// <inheritdoc/>
        public IEntity AsEntity(dynamic dynamicEntity) => DynCodeHelper.AsEntity(dynamicEntity);

        /// <inheritdoc/>
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => DynCodeHelper.AsDynamic(entities);

        #endregion



        #region Data Source Stuff

        /// <inheritdoc/>
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => DynCodeHelper.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc/>
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => DynCodeHelper.CreateSource<T>(inStream);

        #endregion

        #region Content, Header, etc. and List
        /// <inheritdoc/>
        public dynamic Content => DynCodeHelper.Content;

        /// <inheritdoc />
        public dynamic Header => DynCodeHelper.Header;

        #endregion



        #region Customize Data, Search, and Purpose

        /// <inheritdoc />
        public virtual void CustomizeData()
        {
        }

        /// <inheritdoc />
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo,
            DateTime beginDate)
        {
        }

        /// <inheritdoc />
        public Purpose Purpose { get; internal set; }

        #endregion


        #region Adam 

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => DynCodeHelper.AsAdam(entity, fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => DynCodeHelper.AsAdam(entity, fieldName);

        #endregion

    }
}
