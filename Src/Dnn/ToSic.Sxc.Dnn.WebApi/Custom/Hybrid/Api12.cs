using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// This is the base class for all custom API Controllers. <br/>
    /// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP for 2sxc 12")]
    [DnnLogExceptions]
    public abstract partial class Api12: DynamicApiController, IDynamicCode, IDynamicWebApi, IHasDynamicCodeRoot
    {
        [PrivateApi] public int CompatibilityLevel => _DynCodeRoot.CompatibilityLevel;

        /// <inheritdoc />
        public IApp App => _DynCodeRoot.App;

        /// <inheritdoc />
        public IBlockDataSource Data => _DynCodeRoot.Data;


        #region AsDynamic implementations
        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => _DynCodeRoot.AsDynamic(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) =>  _DynCodeRoot.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) =>  _DynCodeRoot.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.AsList(list);

        #endregion


        #region CreateSource implementations

        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            =>  _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc />
	    public T CreateSource<T>(IDataStream inStream) where T : IDataSource 
            => _DynCodeRoot.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List

        /// <inheritdoc />
        public dynamic Content => _DynCodeRoot.Content;

        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot.Header;


        #endregion


        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => _DynCodeRoot.AsAdam(AsEntity(entity), fieldName);

        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => _DynCodeRoot.AsAdam(entity, fieldName);


        /// <inheritdoc />
        public new ToSic.Sxc.Adam.IFile SaveInAdam(string dontRelyOnParameterOrder = ToSic.Eav.Parameters.Protector,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
            => base.SaveInAdam(dontRelyOnParameterOrder, stream, fileName, contentType, guid, field, subFolder);

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01

        /// <inheritdoc />
        public ILinkHelper Link => _DynCodeRoot?.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => _DynCodeRoot?.Edit;

        #endregion

        #region RunContext WiP

        public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;

        [PrivateApi("WIP 12.02")]
        public dynamic Resources => _DynCodeRoot.Resources;

        [PrivateApi("WIP 12.02")]
        public dynamic Settings => _DynCodeRoot.Settings;

        #endregion
    }
}
