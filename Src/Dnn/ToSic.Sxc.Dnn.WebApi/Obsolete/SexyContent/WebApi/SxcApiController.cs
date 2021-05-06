using System;
using System.Collections.Generic;
using ToSic.Eav.DataSources;
using System.IO;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Compatibility.Sxc;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

// ReSharper disable InheritdocInvalidUsage

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.WebApi
{
    /// <inheritdoc cref="SxcApiControllerBase" />
    /// <summary>
    /// This is the base class for API Controllers which need the full context
    /// incl. the current App, DNN, Data, etc.
    /// For others, please use the SxiApiControllerBase, which doesn't have all that, and is usually then
    /// safer because it can't accidentally mix the App with a different appId in the params
    /// </summary>
    [DnnLogExceptions]
    public abstract partial class SxcApiController : 
        DynamicApiController, 
        IDnnDynamicWebApi,
        IDynamicWebApi, 
        IDynamicCodeBeforeV10,
#pragma warning disable 618
        IAppAndDataHelpers
#pragma warning restore 618
    {
        public new IDnnContext Dnn => base.Dnn;

        public SxcHelper Sxc => _sxc ?? (_sxc = new SxcHelper(_DynCodeRoot?.Block?.Context?.UserMayEdit ?? false));
        private SxcHelper _sxc;

        /// <summary>
        /// Old API - probably never used, but we shouldn't remove it as we could break some existing code out there
        /// </summary>
        [PrivateApi] public IBlock Block => GetBlock();
        [PrivateApi] public int CompatibilityLevel => _DynCodeRoot.CompatibilityLevel;

        /// <inheritdoc />
        public IApp App => _DynCodeRoot.App;

        /// <inheritdoc />
        public IBlockDataSource Data => _DynCodeRoot.Data;


        #region AsDynamic implementations

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        [PrivateApi("old api, only available in old API controller")]
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => _DynCodeRoot.AsDynamic(entityKeyValuePair.Value);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => _DynCodeRoot.AsList(stream.List);

        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) =>  _DynCodeRoot.AsEntity(dynamicEntity);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) =>  _DynCodeRoot.AsList(entities);
        #endregion

        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10
        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => _DynCodeRoot.AsDynamic(entity as IEntity);


        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => _DynCodeRoot.AsDynamic(entityKeyValuePair.Value as IEntity);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => _DynCodeRoot.AsList(entities.Cast<IEntity>());
        #endregion


        #region CreateSource implementations
        [Obsolete]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
	        ILookUpEngine lookUpEngine = null)
	        => new DynamicCodeObsolete(_DynCodeRoot).CreateSource(typeName, inSource, lookUpEngine);

        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            =>  _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

	    public T CreateSource<T>(IDataStream inStream) where T : IDataSource 
            => _DynCodeRoot.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List
        /// <summary>
        /// content item of the current view
        /// </summary>
        public dynamic Content => _DynCodeRoot.Content;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use Content.Presentation instead")]
        public dynamic Presentation => _DynCodeRoot.Content?.Presentation;

        public dynamic Header => _DynCodeRoot.Header;

        [Obsolete("use Header instead")]
	    public dynamic ListContent => _DynCodeRoot.Header;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use Header.Presentation instead")]
	    public dynamic ListPresentation => _DynCodeRoot.Header?.Presentation;

        [Obsolete("This is an old way used to loop things. Use Data[\"Default\"] instead. Will be removed in 2sxc v10")]
        public List<Element> List => new DynamicCodeObsolete(_DynCodeRoot).ElementList;

        #endregion


        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
	        => _DynCodeRoot.AsAdam(AsEntity(entity), fieldName);

        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => _DynCodeRoot.AsAdam(entity, fieldName);


        /// <summary>
        /// Save a file from a stream (usually an upload from the browser) into an adam-field
        /// </summary>
        /// <param name="dontRelyOnParameterOrder">ensure that all parameters use names, so the api can change in future</param>
        /// <param name="stream">the stream</param>
        /// <param name="fileName">file name to save to</param>
        /// <param name="contentType">content-type of the target item (important for security checks)</param>
        /// <param name="guid"></param>
        /// <param name="field"></param>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        public new Sxc.Adam.IFile SaveInAdam(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
            => base.SaveInAdam(dontRelyOnParameterOrder, stream, fileName, contentType, guid, field, subFolder);

        #endregion

        #region Link & Edit - added in 2sxc 10.01
        public ILinkHelper Link => _DynCodeRoot?.Link;
        public IInPageEditingSystem Edit => _DynCodeRoot?.Edit;

        #endregion

        #region RunContext - new in 11.08 or similar, not implemented in old base classes

        public ICmsContext CmsContext => _DynCodeRoot.CmsContext;

        #endregion
    }
}
