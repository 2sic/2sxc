using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Compatibility.Sxc;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;

// ReSharper disable InheritdocInvalidUsage

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.WebApi
{
    /// <inheritdoc cref="DynamicApiController" />
    /// <summary>
    /// This is the base class for API Controllers which need the full context
    /// incl. the current App, DNN, Data, etc.
    /// For others, please use the SxiApiControllerBase, which doesn't have all that, and is usually then
    /// safer because it can't accidentally mix the App with a different appId in the params
    /// </summary>
    [DnnLogExceptions]
    [Obsolete("This will continue to work, but you should use the Custom.Hybrid.Api14 or Custom.Dnn.Api12 instead.")]
    [PrivateApi("This was the official base class a long time ago, Name & APIs must remain stable")]
    [DefaultToNewtonsoftForHttpJson]
    public abstract partial class SxcApiController : 
        DynamicApiController, 
        IDnnDynamicWebApi,
        ICreateInstance,
        IDynamicWebApi, 
        IDynamicCodeBeforeV10,
#pragma warning disable 618
        IAppAndDataHelpers,
#pragma warning restore 618
        IHasCodeLog
    {
        protected SxcApiController() : base("OldApi") { }

        public new IDnnContext Dnn => base.Dnn;

        [Obsolete]
        [PrivateApi]
        public SxcHelper Sxc => _sxc ?? (_sxc = new SxcHelper(_DynCodeRoot?.Block?.Context?.UserMayEdit ?? false, SysHlp.GetService<IConvertToEavLight> ()));
        [Obsolete]
        private SxcHelper _sxc;

        /// <summary>
        /// Old API - probably never used, but we shouldn't remove it as we could break some existing code out there
        /// </summary>
        [PrivateApi] public IBlock Block => SysHlp.GetBlockAndContext(Request).LoadBlock();

        [PrivateApi] public int CompatibilityLevel => Constants.CompatibilityLevel9Old;

        /// <inheritdoc cref="IDynamicCode.App" />
        public IApp App => _DynCodeRoot.App;

        /// <inheritdoc cref="IDynamicCode.Data" />
        public IContextData Data => _DynCodeRoot.Data;


        #region AsDynamic implementations

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsC.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsC.AsDynamicInternal(dynamicEntity);

        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsC.MergeDynamic(entities);

        /// <inheritdoc />
        [PrivateApi("old api, only available in old API controller")]
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => _DynCodeRoot.AsC.AsDynamic(entityKeyValuePair.Value);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => _DynCodeRoot.AsC.AsDynamicList(stream.List);

        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) =>  _DynCodeRoot.AsC.AsEntity(dynamicEntity);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) =>  _DynCodeRoot.AsC.AsDynamicList(entities);
        #endregion

        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10
        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => _DynCodeRoot.AsC.AsDynamic(entity as IEntity);


        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => _DynCodeRoot.AsC.AsDynamic(entityKeyValuePair.Value as IEntity);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => _DynCodeRoot.AsC.AsDynamicList(entities.Cast<IEntity>());
        #endregion


        #region CreateSource implementations
        [Obsolete]
        [PrivateApi]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine configurationProvider = null)
	        => new DynamicCodeObsolete(_DynCodeRoot).CreateSource(typeName, inSource, configurationProvider);

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
	    public T CreateSource<T>(IDataStream source) where T : IDataSource 
            => _DynCodeRoot.CreateSource<T>(source);

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

        /// <inheritdoc cref="IDynamicCode.AsAdam" />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot.AsAdam(item, fieldName);

        /// <summary>
        /// Save a file from a stream (usually an upload from the browser) into an adam-field
        /// </summary>
        /// <param name="noParamOrder">ensure that all parameters use names, so the api can change in future</param>
        /// <param name="stream">the stream</param>
        /// <param name="fileName">file name to save to</param>
        /// <param name="contentType">content-type of the target item (important for security checks)</param>
        /// <param name="guid"></param>
        /// <param name="field"></param>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        public new Sxc.Adam.IFile SaveInAdam(string noParamOrder = Eav.Parameters.Protector,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
            => base.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

        #endregion

        #region CreateInstance

        string IGetCodePath.CreateInstancePath { get; set; }

        /// <inheritdoc cref="ICreateInstance.CreateInstance"/>
        public dynamic CreateInstance(string virtualPath, string noParamOrder = ToSic.Eav.Parameters.Protector, string name = null, string relativePath = null, bool throwOnError = true)
            => _DynCodeRoot.CreateInstance(virtualPath, noParamOrder, name, ((IGetCodePath)this).CreateInstancePath, throwOnError);

        #endregion

        #region Link & Edit - added in 2sxc 10.01
        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot?.Link;
        /// <inheritdoc cref="IDynamicCode.Edit" />
        public IEditService Edit => _DynCodeRoot?.Edit;

        #endregion

        #region CmsContext, Resources and Settings

        /// <inheritdoc cref="IDynamicCode.CmsContext" />
        public ICmsContext CmsContext => _DynCodeRoot.CmsContext;

        ///// <inheritdoc />
        //public dynamic Resources => _DynCodeRoot.Resources;

        ///// <inheritdoc />
        //public dynamic Settings => _DynCodeRoot.Settings;

        #endregion

        #region IHasLog

        /// <inheritdoc />
        public new ICodeLog Log => _codeLog.Get(() => new CodeLog(base.Log));
        private readonly GetOnce<ICodeLog> _codeLog = new GetOnce<ICodeLog>();

        [PrivateApi] ILog IHasLog.Log => base.Log;

        #endregion
    }
}
