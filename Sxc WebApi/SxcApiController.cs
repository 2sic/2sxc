using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.Interfaces;
using ToSic.Eav.ValueProvider;
using ToSic.Eav.WebApi;
using ToSic.SexyContent.Adam;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.SexyContent.Security;
using ToSic.SexyContent.WebApi.Dnn;

namespace ToSic.SexyContent.WebApi
{
	// disabled - as it is now accessible from many other modules and sometimes without a specific module [SupportedModules("2sxc,2sxc-app")]
    [SxcWebApiExceptionHandling]
    public abstract class SxcApiController : DnnApiControllerWithFixes, IAppAndDataHelpers
    {
        private AppAndDataHelpers AppAndDataHelpers => _appAndDataHelpers ?? (_appAndDataHelpers = new AppAndDataHelpers(SxcContext, SxcContext?.ModuleInfo));

        private AppAndDataHelpers _appAndDataHelpers;

	    // Sexy object should not be accessible for other assemblies - just internal use
        internal SxcInstance SxcContext => _instanceContext ?? (_instanceContext = Helpers.GetSxcOfApiRequest(Request, true));
        private SxcInstance _instanceContext;

        //protected IEnvironment Env = new Environment.DnnEnvironment();

        #region AppAndDataHelpers implementation

        public DnnHelper Dnn => AppAndDataHelpers.Dnn;

	    public SxcHelper Sxc => AppAndDataHelpers.Sxc;

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
                return _app = (App) Environment.Dnn7.Factory.App(appId, new Environment.Dnn7.PagePublishing().IsVersioningEnabled(this.Dnn.Module.ModuleID));
            }
        }
        private App _app;

        public ViewDataSource Data => AppAndDataHelpers.Data;

	    /// <summary>
        /// Transform a IEntity to a DynamicEntity as dynamic object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(IEntity entity) => AppAndDataHelpers.AsDynamic(entity);
        

        /// <summary>
        /// Makes sure a dynamicEntity could be wrapped in AsDynamic()
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(dynamic dynamicEntity) =>  AppAndDataHelpers.AsDynamic(dynamicEntity);

        /// <summary>
        /// Returns the value of a KeyValuePair as DynamicEntity
        /// </summary>
        /// <param name="entityKeyValuePair"></param>
        /// <returns></returns>
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) =>  AppAndDataHelpers.AsDynamic(entityKeyValuePair.Value);

	    /// <summary>
	    /// In case AsDynamic is used with Data["name"]
	    /// </summary>
	    /// <param name="stream"></param>
	    /// <returns></returns>
	    public IEnumerable<dynamic> AsDynamic(IDataStream stream) =>  AppAndDataHelpers.AsDynamic(stream.List);

        /// <summary>
        /// In case AsDynamic is used with Data["name"].List
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IDictionary<int, IEntity> list) =>  AppAndDataHelpers.AsDynamic(list);

        /// <summary>
        /// Transform a DynamicEntity dynamic object back to a IEntity instance
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public IEntity AsEntity(dynamic dynamicEntity) =>  AppAndDataHelpers.AsEntity(dynamicEntity);

        /// <summary>
        /// Returns a list of DynamicEntities
        /// </summary>
        /// <param name="entities">List of entities</param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) =>  AppAndDataHelpers.AsDynamic(entities);

	    public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
	        IValueCollectionProvider configurationProvider = null)
	        => AppAndDataHelpers.CreateSource(typeName, inSource, configurationProvider);

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
            =>  AppAndDataHelpers.CreateSource<T>(inSource, configurationProvider);

	    /// <summary>
	    /// Create a source with initial stream to attach...
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="inStream"></param>
	    /// <returns></returns>
	    public T CreateSource<T>(IDataStream inStream) => AppAndDataHelpers.CreateSource<T>(inStream);

		public dynamic Content => AppAndDataHelpers.Content;

	    public dynamic Presentation => AppAndDataHelpers.Presentation;

	    public dynamic ListContent => AppAndDataHelpers.ListContent;

	    public dynamic ListPresentation => AppAndDataHelpers.ListPresentation;

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
            var zid = Env.ZoneMapper.GetZoneId(PortalSettings.PortalId); // ZoneHelpers.GetZoneId(PortalSettings.PortalId);
            //if (zid == null)
            //    throw new Exception("zone not found");

            // get app from appname
            var aid = AppHelpers.GetAppIdFromGuidName(zid, appPath, true);
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
            Guid ctGuid;

            // only check permissions on type if the type has a GUID as static-id
            var staticNameIsGuid = Guid.TryParse(ct.StaticName, out ctGuid);
            // Check permissions in 2sxc - or check if the user has admin-right (in which case he's always granted access for these types of content)
            if (staticNameIsGuid 
                && new PermissionController(zoneId, appId.Value, ctGuid, specificItem, contextMod)
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
            string helpText = " See http://2sxc.org/help" + (tags == "" ? "" : "?tag=" + tags);
            throw new HttpResponseException(new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(message + helpText),
                ReasonPhrase = "Error in 2sxc Content API - not allowed"
            });
        }

        #endregion
    }
}
