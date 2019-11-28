using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using ToSic.Eav.DataSources;
using Factory = ToSic.Eav.Factory;
using ToSic.Sxc.Adam.WebApi;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.WebApi;
using DynamicCodeHelper = ToSic.Sxc.Dnn.DynamicCodeHelper;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;

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
    [SxcWebApiExceptionHandling]
    public abstract class DynamicApiController : SxcApiControllerBase //, IDynamicWebApi, IDynamicCodeBeforeV10
    {
        #region constructor

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            // Note that the SxcBlock is created by the BaseClass, if it's detectable. Otherwise it's null
            DynCodeHelpers = new DynamicCodeHelper(CmsBlock, CmsBlock?.Log ?? Log);

            // In case SxcBlock was null, there is no instance, but we may still need the app
            if (DynCodeHelpers.App == null)
                TryToAttachAppFromUrlParams();

            // must run this after creating AppAndDataHelpers
            controllerContext.Request.Properties.Add(Constants.DnnContextKey, Dnn); 

            if(controllerContext.Request.Properties.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var value))
                SharedCodeVirtualRoot = value as string;
        }
        #endregion

        private DynamicCodeHelper DynCodeHelpers { get; set; }

        public IDnnContext Dnn => DynCodeHelpers.Dnn;

        //[PrivateApi]
        //public SxcHelper Sxc => DynCodeHelpers.Sxc;

        ///// <inheritdoc />
        //public IApp App => DynCodeHelpers.App;

        private void TryToAttachAppFromUrlParams()
        {
            var wrapLog = Log.Call("TryToAttachAppFromUrlParams");
            var found = false;
            try
            {
                var routeAppPath = Route.AppPathOrNull(Request.GetRouteData());
                var appId = AppFinder.GetCurrentAppIdFromPath(routeAppPath).AppId;
                // Look up if page publishing is enabled - if module context is not available, always false
                var publish = Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
                var publishingEnabled = Dnn.Module != null && publish.IsEnabled(Dnn.Module.ModuleID);
                var app = Environment.Dnn7.Factory.App(appId, publishingEnabled) as IApp;
                DynCodeHelpers.LateAttachApp(app);
                found = true;
            } catch { /* ignore */ }

            wrapLog(found.ToString());
        }

        ///// <inheritdoc />
        //public IBlockDataSource Data => DynCodeHelpers.Data;


        #region AsDynamic implementations

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => DynCodeHelpers.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) =>  DynCodeHelpers.AsDynamic(dynamicEntity);

        //// todo: only in "old" controller, not in new one
        ///// <inheritdoc />
        //public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) =>  DynCodeHelpers.AsDynamic(entityKeyValuePair.Value);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) =>  DynCodeHelpers.AsDynamic(stream.List);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) =>  DynCodeHelpers.AsEntity(dynamicEntity);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) =>  DynCodeHelpers.AsDynamic(entities);
        #endregion

        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10
        //[PrivateApi]
        //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        //public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => DynCodeHelpers.AsDynamic(entity);


        //[PrivateApi]
        //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        //public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => DynCodeHelpers.AsDynamic(entityKeyValuePair.Value);

        //[PrivateApi]
        //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        //public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => DynCodeHelpers.AsDynamic(entities);
        #endregion


     //   #region CreateSource implementations
     //   public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
	    //    ITokenListFiller configurationProvider = null)
	    //    => DynCodeHelpers.CreateSource(typeName, inSource, configurationProvider);

     //   public T CreateSource<T>(IDataSource inSource = null, ITokenListFiller configurationProvider = null)
     //       where T : IDataSource
     //       =>  DynCodeHelpers.CreateSource<T>(inSource, configurationProvider);

	    //public T CreateSource<T>(IDataStream inStream) where T : IDataSource 
     //       => DynCodeHelpers.CreateSource<T>(inStream);

     //   #endregion

        #region Content, Presentation & List
     //   /// <summary>
     //   /// content item of the current view
     //   /// </summary>
     //   public dynamic Content => DynCodeHelpers.Content;

     //   /// <summary>
     //   /// presentation item of the content-item. 
     //   /// </summary>
     //   [Obsolete("please use Content.Presentation instead")]
     //   public dynamic Presentation => DynCodeHelpers.Content?.Presentation;

     //   public dynamic Header => DynCodeHelpers.Header;

     //   [Obsolete("use Header instead")]
	    //public dynamic ListContent => DynCodeHelpers.Header;

     //   /// <summary>
     //   /// presentation item of the content-item. 
     //   /// </summary>
     //   [Obsolete("please use Header.Presentation instead")]
	    //public dynamic ListPresentation => DynCodeHelpers.Header?.Presentation;

     //   [Obsolete("This is an old way used to loop things. Use Data[\"Default\"] instead. Will be removed in 2sxc v10")]
     //   public List<Element> List => DynCodeHelpers.List;

        #endregion


        #region Adam

        ///// <inheritdoc />
        //public IFolder AsAdam(IDynamicEntity entity, string fieldName)
	       // => DynCodeHelpers.AsAdam(AsEntity(entity), fieldName);

        ///// <inheritdoc />
        //public IFolder AsAdam(IEntity entity, string fieldName) => DynCodeHelpers.AsAdam(entity, fieldName);


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
        public Sxc.Adam.IFile SaveInAdam(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, 
            Stream stream = null, 
            string fileName = null, 
            string contentType = null, 
            Guid? guid = null, 
            string field = null,
            string subFolder = "")
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "SaveInAdam", 
                $"{nameof(stream)},{nameof(fileName)},{nameof(contentType)},{nameof(guid)},{nameof(field)},{nameof(subFolder)} (optional)");

            if(stream == null || fileName == null || contentType == null || guid == null || field == null)
                throw new Exception();

            var feats = new[]{FeatureIds.UseAdamInWebApi, FeatureIds.PublicUpload};
            if (!Features.EnabledOrException(feats, "can't save in ADAM", out var exp))
                throw exp;

            return new AdamUploader(CmsBlock, 
                CmsBlock.Block.AppId, // 2019-11-09 not nullable any more ?? throw new Exception("can't save in adam - full context not available"), 
                Log)
                .UploadOne(stream, fileName, contentType, guid.Value, field, subFolder, false, true);
        }

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01
        //public ILinkHelper Link => DynCodeHelpers?.Link;
        //public IInPageEditingSystem Edit => DynCodeHelpers?.Edit;

        #endregion


        public string SharedCodeVirtualRoot { get; set; }

        public dynamic CreateInstance(string virtualPath, 
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null, 
            string relativePath = null, 
            bool throwOnError = true) =>
            DynCodeHelpers.CreateInstance(virtualPath, dontRelyOnParameterOrder, name,
                SharedCodeVirtualRoot, throwOnError);
    }
}
