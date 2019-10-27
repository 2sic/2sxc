using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.DataSources;
using ToSic.Eav.Interfaces;
using ToSic.Eav.ValueProvider;
using ToSic.Sxc.Adam;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Razor.Helpers;
using Factory = ToSic.Eav.Factory;
using ToSic.Sxc.Adam.WebApi;
using System.IO;
using ToSic.Eav.Configuration;
using ToSic.Sxc.Interfaces;
using ToSic.SexyContent.WebApi.AutoDetectContext;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Interfaces;
using File = ToSic.Sxc.Adam.File;

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
    public abstract class SxcApiController : SxcApiControllerBase, Sxc.Interfaces.IAppAndDataHelpers, IHasDnnContext
    {
        #region constructor

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            // Note that the SxcInstance is created by the BaseClass, if it's detectable. Otherwise it's null
            DnnAppAndDataHelpers = new DnnAppAndDataHelpers(SxcInstance, SxcInstance?.Log ?? Log);

            // In case SxcInstance was null, there is no instance, but we may still need the app
            if (DnnAppAndDataHelpers.App == null)
                TryToAttachAppFromUrlParams();

            // must run this after creating AppAndDataHelpers
            controllerContext.Request.Properties.Add(Constants.DnnContextKey, Dnn); 

            if(controllerContext.Request.Properties.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var value))
                SharedCodeVirtualRoot = value as string;
        }
        #endregion

        private DnnAppAndDataHelpers DnnAppAndDataHelpers { get; set; }

        public DnnHelper Dnn => DnnAppAndDataHelpers.Dnn;

        public SxcHelper Sxc => DnnAppAndDataHelpers.Sxc;

        /// <inheritdoc />
        public App App => DnnAppAndDataHelpers.App;

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
                var app = (App) Environment.Dnn7.Factory.App(appId, publishingEnabled);
                DnnAppAndDataHelpers.LateAttachApp(app);
                found = true;
            } catch { /* ignore */ }

            wrapLog(found.ToString());
        }

        /// <inheritdoc />
        public ViewDataSource Data => DnnAppAndDataHelpers.Data;


        #region AsDynamic implementations
        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => DnnAppAndDataHelpers.AsDynamic(entity);


        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) =>  DnnAppAndDataHelpers.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) =>  DnnAppAndDataHelpers.AsDynamic(entityKeyValuePair.Value);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) =>  DnnAppAndDataHelpers.AsDynamic(stream.List);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) =>  DnnAppAndDataHelpers.AsEntity(dynamicEntity);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) =>  DnnAppAndDataHelpers.AsDynamic(entities);
        #endregion

        #region CreateSource implementations
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
	        IValueCollectionProvider configurationProvider = null)
	        => DnnAppAndDataHelpers.CreateSource(typeName, inSource, configurationProvider);

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
            =>  DnnAppAndDataHelpers.CreateSource<T>(inSource, configurationProvider);

	    /// <inheritdoc />
	    public T CreateSource<T>(IDataStream inStream) => DnnAppAndDataHelpers.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List
        /// <summary>
        /// content item of the current view
        /// </summary>
        public dynamic Content => DnnAppAndDataHelpers.Content;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use Content.Presentation instead")]
        public dynamic Presentation => DnnAppAndDataHelpers.Content?.Presentation;

	    public dynamic ListContent => DnnAppAndDataHelpers.ListContent;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use ListContent.Presentation instead")]
	    public dynamic ListPresentation => DnnAppAndDataHelpers.ListContent?.Presentation;

        [Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in 2sxc v10")]
        public List<Element> List => DnnAppAndDataHelpers.List;

	    #endregion


        #region Adam

	    /// <summary>
	    /// Provides an Adam instance for this item and field
	    /// </summary>
	    /// <param name="entity">The entity, often Content or similar</param>
	    /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
	    /// <returns>An Adam object for navigating the assets</returns>
	    public FolderOfField AsAdam(IDynamicEntity entity, string fieldName)
	        => DnnAppAndDataHelpers.AsAdam(AsEntity(entity), fieldName);

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public FolderOfField AsAdam(IEntity entity, string fieldName) => DnnAppAndDataHelpers.AsAdam(entity, fieldName);


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
        public File SaveInAdam(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, 
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

            return new AdamUploader(SxcInstance, 
                SxcInstance.AppId ?? throw new Exception("can't save in adam - full context not available"), 
                Log)
                .UploadOne(stream, fileName, contentType, guid.Value, field, subFolder, false, true);
        }

        #endregion

        #region Link & Edit - added in 2sxc 10.01
        public ILinkHelper Link => DnnAppAndDataHelpers?.Link;
        public IInPageEditingSystem Edit => DnnAppAndDataHelpers?.Edit;

        #endregion


        public string SharedCodeVirtualRoot { get; set; }

        public dynamic CreateInstance(string virtualPath, 
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null, 
            string relativePath = null, 
            bool throwOnError = true) =>
            DnnAppAndDataHelpers.CreateInstance(virtualPath, dontRelyOnParameterOrder, name,
                SharedCodeVirtualRoot, throwOnError);
    }
}
