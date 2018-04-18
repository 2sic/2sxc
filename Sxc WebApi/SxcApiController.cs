using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.DataSources;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.ValueProvider;
using ToSic.Sxc.Adam;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.Razor.Helpers;
using Factory = ToSic.Eav.Factory;

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
    public abstract class SxcApiController : SxcApiControllerBase, IAppAndDataHelpers
    {
        #region constructor

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            // Note that the SxcInstance is created by the BaseClass, if it's detectable. Otherwise it's null
            DnnAppAndDataHelpers = new DnnAppAndDataHelpers(SxcInstance, SxcInstance?.EnvInstance, SxcInstance?.Log ?? Log);
            controllerContext.Request.Properties.Add(Constants.DnnContextKey, Dnn); // must run after creating AppAndDataHelpers
        }
        #endregion

        private DnnAppAndDataHelpers DnnAppAndDataHelpers { get; set; }

        #region AppAndDataHelpers implementation

        public DnnHelper Dnn => DnnAppAndDataHelpers.Dnn;

        public SxcHelper Sxc => DnnAppAndDataHelpers.Sxc;

        /// <inheritdoc />
        public App App
        {
            get
            {
                // try already-retrieved
                if (_app != null)
                    return _app;

                // try "normal" case with instance context
                if (SxcInstance != null)
                    return _app = DnnAppAndDataHelpers.App;

                var routeAppPath = Request.GetRouteData().Values["apppath"]?.ToString();
                var appId = GetCurrentAppIdFromPath(routeAppPath);
                // Look up if page publishing is enabled - if module context is not availabe, always false
                var publish = Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
                var publishingEnabled = Dnn.Module != null && publish.IsEnabled(Dnn.Module.ModuleID);
                return _app = (App) Environment.Dnn7.Factory.App(appId, publishingEnabled);
            }
        }
        private App _app;

        /// <inheritdoc />
        public ViewDataSource Data => DnnAppAndDataHelpers.Data;

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

	    public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
	        IValueCollectionProvider configurationProvider = null)
	        => DnnAppAndDataHelpers.CreateSource(typeName, inSource, configurationProvider);

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
            =>  DnnAppAndDataHelpers.CreateSource<T>(inSource, configurationProvider);

	    /// <inheritdoc />
	    public T CreateSource<T>(IDataStream inStream) => DnnAppAndDataHelpers.CreateSource<T>(inStream);

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
	    public FolderOfField AsAdam(DynamicEntity entity, string fieldName)
	        => DnnAppAndDataHelpers.AsAdam(AsEntity(entity), fieldName);

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public FolderOfField AsAdam(IEntity entity, string fieldName) => DnnAppAndDataHelpers.AsAdam(entity, fieldName);
        #endregion

        #region App-Helpers for anonyous access APIs

        internal int GetCurrentAppIdFromPath(string appPath)
        {
            // check zone
            var zid = Env.ZoneMapper.GetZoneId(PortalSettings.PortalId);

            // get app from appname
            var aid = AppHelpers.GetAppIdFromGuidName(zid, appPath, true);
            Log.Add($"find app by path:{appPath}, found a:{aid}");
            return aid;
        }
        #endregion

        #region Security Checks 

        /// <summary>
        /// Check if a user may do something - and throw an error if the permission is not given
        /// </summary>
        internal void PerformSecurityCheck(int appId, string contentType, Grants grant,
            ModuleInfo module, App app, IEntity specificItem = null)
            => new Security(PortalSettings, Log).FindCtCheckSecurityOrThrow(appId,
                contentType,
                new List<Grants> {grant},
                specificItem,
                module,
                app);

        #endregion

    }
}
