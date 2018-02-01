using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using ToSic.Eav;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.DataSources;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.ValueProvider;
using ToSic.Eav.WebApi;
using ToSic.SexyContent.Adam;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.SexyContent.WebApi.Dnn;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent.WebApi
{
	// this is accessible from many non-2sxc modules so no [SupportedModules("2sxc,2sxc-app")]
    [SxcWebApiExceptionHandling]
    public abstract class SxcApiController : DnnApiControllerWithFixes, IAppAndDataHelpers
    {
        #region constructor

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Log.Rename("2sApiC");
            SxcContext = Helpers.GetSxcOfApiRequest(Request, true, Log);
            AppAndDataHelpers = new AppAndDataHelpers(SxcContext, SxcContext?.InstanceInfo, SxcContext?.Log ?? Log);
            controllerContext.Request.Properties.Add(Constants.DnnContextKey, Dnn); // must run after creating AppAndDataHelpers
        }
        #endregion

        private AppAndDataHelpers AppAndDataHelpers { get; set; }

	    // Sexy object should not be accessible for other assemblies - just internal use
        internal SxcInstance SxcContext { get; private set; }

        #region AppAndDataHelpers implementation

        /// <inheritdoc />
        public DnnHelper Dnn => AppAndDataHelpers.Dnn;

	    public SxcHelper Sxc => AppAndDataHelpers.Sxc;

        /// <inheritdoc />
        public App App
        {
            get
            {
                // try already-retrieved
                if (_app != null)
                    return _app;

                // try "normal" case with instance context
                if (SxcContext != null)
                    return _app = AppAndDataHelpers.App;

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
        public ViewDataSource Data => AppAndDataHelpers.Data;

	    /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => AppAndDataHelpers.AsDynamic(entity);


        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) =>  AppAndDataHelpers.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) =>  AppAndDataHelpers.AsDynamic(entityKeyValuePair.Value);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) =>  AppAndDataHelpers.AsDynamic(stream.List);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) =>  AppAndDataHelpers.AsEntity(dynamicEntity);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) =>  AppAndDataHelpers.AsDynamic(entities);

	    public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
	        IValueCollectionProvider configurationProvider = null)
	        => AppAndDataHelpers.CreateSource(typeName, inSource, configurationProvider);

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
            =>  AppAndDataHelpers.CreateSource<T>(inSource, configurationProvider);

	    /// <inheritdoc />
	    public T CreateSource<T>(IDataStream inStream) => AppAndDataHelpers.CreateSource<T>(inStream);

        /// <summary>
        /// content item of the current view
        /// </summary>
        public dynamic Content => AppAndDataHelpers.Content;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use Content.Presentation instead")]
        public dynamic Presentation => AppAndDataHelpers.Content?.Presentation;

	    public dynamic ListContent => AppAndDataHelpers.ListContent;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use ListContent.Presentation instead")]
	    public dynamic ListPresentation => AppAndDataHelpers.ListContent?.Presentation;

        [Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in 2sxc v10")]
        public List<Element> List => AppAndDataHelpers.List;

	    #endregion


        #region Adam

	    /// <summary>
	    /// Provides an Adam instance for this item and field
	    /// </summary>
	    /// <param name="entity">The entity, often Content or similar</param>
	    /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
	    /// <returns>An Adam object for navigating the assets</returns>
	    public AdamNavigator AsAdam(DynamicEntity entity, string fieldName)
	        => AppAndDataHelpers.AsAdam(AsEntity(entity), fieldName);

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public AdamNavigator AsAdam(IEntity entity, string fieldName) => AppAndDataHelpers.AsAdam(entity, fieldName);
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
        /// <param name="contentType"></param>
        /// <param name="grant"></param>
        /// <param name="autoAllowAdmin"></param>
        /// <param name="specificItem"></param>
        /// <param name="useContext"></param>
        /// <param name="appId"></param>
        internal void PerformSecurityCheck(string contentType, PermissionGrant grant, bool autoAllowAdmin = false, IEntity specificItem = null, bool useContext = true, int? appId = null)
        {
            Log.Add($"security check for type:{contentType}, grant:{grant}, autoAdmin:{autoAllowAdmin}, useContext:{useContext}, app:{appId}, item:{specificItem?.EntityId}");
            // make sure we have the right appId, zoneId and module-context
            var contextMod = useContext ? Dnn.Module : null;
            var zoneId = useContext ? App?.ZoneId : null;   // App is null, when accessing admin-ui from un-initialized module
            if (useContext) appId = App?.AppId ?? appId;
            if (!useContext) autoAllowAdmin = false; // auto-check not possible when not using context


            if (!appId.HasValue)
                throw new Exception("app id doesn't have value, and apparently didn't get it from context either");

            // Check if we can find this content-type
            var ctc = new ContentTypeController();
            ctc.SetAppIdAndUser(appId.Value);

            var cache = DataSource.GetCache(zoneId, appId);
            var ct = cache.GetContentType(contentType);

            if (ct == null)
            {
                ThrowHttpError(HttpStatusCode.NotFound, "Could not find Content Type '" + contentType + "'.",
                    "content-types");
                return;
            }

            // Check if the content-type has a GUID as name - only these can have permission assignments

            // only check permissions on type if the type has a GUID as static-id
            var staticNameIsGuid = Guid.TryParse(ct.StaticName, out var ctGuid);
            // Check permissions in 2sxc - or check if the user has admin-right (in which case he's always granted access for these types of content)
            if (staticNameIsGuid 
                && new DnnPermissionController(ct, specificItem, Log, new DnnInstanceInfo(contextMod))
                    .UserMay(grant))
                return;

            // if initial test couldn't be done (non-guid) or failed, test for admin-specifically
            if (autoAllowAdmin 
                && DotNetNuke.Security.Permissions.ModulePermissionController.CanAdminModule(contextMod))
                return;

            // if the cause was not-admin and not testable, report better error
            if (!staticNameIsGuid)
                ThrowHttpError(HttpStatusCode.Unauthorized,
                    "Content Type '" + contentType + "' is not a standard Content Type - no permissions possible.");

            // final case: simply not allowed
            ThrowHttpError(HttpStatusCode.Unauthorized,
                "Request not allowed. User needs permissions to " + grant + " for Content Type '" + contentType + "'.",
                "permissions");
        }

        /// <summary>
        /// Throw a correct HTTP error with the right error-numbr. This is important for the JavaScript which changes behavior & error messages based on http status code
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="message"></param>
        /// <param name="tags"></param>
        private static void ThrowHttpError(HttpStatusCode httpStatusCode, string message, string tags = "")
        {
            var helpText = " See http://2sxc.org/help" + (tags == "" ? "" : "?tag=" + tags);
            throw new HttpResponseException(new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(message + helpText),
                ReasonPhrase = "Error in 2sxc Content API - not allowed"
            });
        }

        #endregion

    }
}
