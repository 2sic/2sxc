using System.Collections.Generic;
using ToSic.Eav.Configuration;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;


// ReSharper disable once CheckNamespace
namespace Custom.Hybrid.Advanced
{
    /// <summary>
    /// The base class for Hybrid Razor-Components in 2sxc 12 <br/>
    /// Provides context objects like CmsContext, helpers like Edit and much more. <br/>
    /// </summary>
    [PublicApi]
    public abstract partial class Razor14<TModel, TServiceKit>
    {

        #region Link, Edit, Dnn, App, Data

        /// <inheritdoc />
        public ILinkService Link => _DynCodeRoot.Link;

        /// <inheritdoc />
        public IEditService Edit => _DynCodeRoot.Edit;

        /// <inheritdoc />
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        [PrivateApi] public int CompatibilityLevel => _DynCodeRoot.CompatibilityLevel;

        /// <inheritdoc />
        public new IApp App => _DynCodeRoot.App;

        /// <inheritdoc />
        public IBlockDataSource Data => _DynCodeRoot.Data;

        #endregion

        #region AsDynamic in many variations

        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => _DynCodeRoot.AsDynamic(json, fallback);

        /// <inheritdoc/>
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsDynamic(entity);

        /// <inheritdoc/>
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsDynamic(dynamicEntity);

        /// <inheritdoc/>
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsDynamic(entities);

        #endregion

        #region AsEntity
        /// <inheritdoc/>
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.AsEntity(dynamicEntity);
        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot.AsList(list);

        #endregion

        #region Convert-Service - Removed it V14!

        #endregion


        #region Data Source Stuff

        /// <inheritdoc/>
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc/>
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inStream);

        [PrivateApi]
        public IDataSource CreateSourceWip(
                string name,
                string noParamOrder = ToSic.Eav.Parameters.Protector,
                IDataSource source = default,
                IConfiguration configuration = default)
            => _DynCodeRoot.CreateSourceWip(name, source: source, configuration: configuration);

        #endregion



        #region Content, Header, etc. and List
        /// <inheritdoc/>
        public dynamic Content => _DynCodeRoot.Content;

        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot.Header;

        #endregion





        #region Adam 

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => _DynCodeRoot.AsAdam(entity, fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => _DynCodeRoot.AsAdam(entity, fieldName);

        #endregion

        #region v11 properties CmsContext

        /// <inheritdoc />
        public ICmsContext CmsContext => _DynCodeRoot.CmsContext;
        #endregion

        #region v12 properties Resources, Settings, Path

        /// <inheritdoc />
        public dynamic Resources => _DynCodeRoot.Resources;

        /// <inheritdoc />
        public dynamic Settings => _DynCodeRoot.Settings;

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        /// <inheritdoc />
        public string Path => VirtualPath;

        #endregion
    }
}
