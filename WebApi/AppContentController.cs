using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using ToSic.Eav;
using ToSic.Eav.DataSources.Caches;
using ToSic.Eav.WebApi;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Security;
using ToSic.SexyContent.Serializers;
using ToSic.SexyContent.WebApi.ToRefactorDeliverCBDataLight;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// todo: security
    /// </summary>
    // [SupportedModules("2sxc,2sxc-app")]
    [AllowAnonymous]
    public class AppContentController : SxcApiController
	{
	    private EntitiesController _entitiesController;

	    private void InitEavAndSerializer(int? appId = null)
	    {
            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
            _entitiesController = new EntitiesController(appId ?? App.AppId);
            _entitiesController.SetUser(Environment.Dnn7.UserIdentity.CurrentUserIdentityToken);

            // only do this if we have a real context - otherwise don't do this
	        if (!appId.HasValue)
	            ((Serializer) _entitiesController.Serializer).Sxc = SxcContext;

	    }

        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, string appPath = null, string cultureCode = null)
        {
            // if app-path specified, use that app, otherwise use from context
            var appId = GetAppIdFromPathOrContext(appPath);

            InitEavAndSerializer(appId);
            PerformSecurityCheck(contentType, PermissionGrant.Read, true, useContext: appPath == null, appId: appId);
            return _entitiesController.GetEntities(contentType, cultureCode);
        }

        private int GetAppIdFromPathOrContext(string appPath)
        {
            var appId = appPath == null || appPath == "auto" ? App.AppId : GetCurrentAppIdFromPath(appPath);
            return appId;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
	    public HttpResponseMessage GetContentBlockData()
        {
            InitEavAndSerializer();
            // Important note: we are NOT supporting url-view switch at the moment for this
            // reason is, that this kind of data-access is fairly special
            // and not recommended for future use cases, where we have the query etc.
            // IF you want to support View-switching in this, do a deep review w/2dm first!
            // - note that it's really not needed, as you can always use a query or something similar instead
            // - not also that if ever you do support view switching, you will need to ensure security checks

            var dataHandler = new GetContentBlockDataLight(SxcContext);

            // must access engine to ensure pre-processing of data has happened, 
            // especially if the cshtml contains a override void CustomizeData()
            SxcContext.GetRenderingEngine(InstancePurposes.PublishData);  

            var dataSource = SxcContext.Data;
            string json;
            if (dataSource.Publish.Enabled)
            {
                var publishedStreams = dataSource.Publish.Streams;
                var streamList = publishedStreams.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                json = dataHandler.GetJsonFromStreams(dataSource, streamList);
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
                { ReasonPhrase = dataHandler.GeneratePleaseEnableDataError(SxcContext.ModuleInfo.ModuleID,
                    SxcContext.ModuleInfo.ModuleTitle)});
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
    }

	    /// <summary>
	    /// Check if a user may do something - and throw an error if the permission is not given
	    /// </summary>
	    /// <param name="contentType"></param>
	    /// <param name="grant"></param>
	    /// <param name="autoAllowAdmin"></param>
	    /// <param name="specificItem"></param>
	    private void PerformSecurityCheck(string contentType, PermissionGrant grant, bool autoAllowAdmin = false, IEntity specificItem = null, bool useContext = true, int? appId = null)
	    {
            // make sure we have the right appId, zoneId and module-context
	        var contextMod = useContext ? Dnn.Module : null;
            var zoneId = useContext ? App.ZoneId as int? : null;
	        if(useContext) appId = App.AppId;
	        if (!useContext) autoAllowAdmin = false; // auto-check not possible when not using context

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
            var staticNameIsGuid = Guid.TryParse(ct.StaticName, out ctGuid);
            if (!staticNameIsGuid)
                ThrowHttpError(HttpStatusCode.Unauthorized,
                    "Content Type '" + contentType + "' is not a standard Content Type - no permissions possible.");

            // Check permissions in 2sxc - or check if the user has admin-right (in which case he's always granted access for these types of content)
            var permissionChecker = new PermissionController(zoneId, appId.Value, ctGuid, specificItem, contextMod);
            var allowed = permissionChecker.UserMay(grant);

            var isAdmin = autoAllowAdmin &&
                          DotNetNuke.Security.Permissions.ModulePermissionController.CanAdminModule(contextMod);

            if (!(allowed || isAdmin))
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

	    /// <summary>
		/// Get Entities with specified AssignmentObjectTypeId and Key
		/// </summary>
		[HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
		public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int assignmentObjectTypeId, Guid keyGuid, string contentType)
		{
            InitEavAndSerializer();
	        return new MetadataController().GetAssignedEntities(assignmentObjectTypeId, "guid", keyGuid.ToString(), contentType);
		}


        #region Delete

        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public void Delete(string contentType, int id)
        {
            InitEavAndSerializer();
            // don't allow type "any" on this
            if(contentType == "any")
                throw new Exception("type any not allowed with id-only, requires guid");

            IEntity itm = App.Data.Out["Default"].List[id]; // pre-fetch for security checks
            contentType = Delete_SharedCode(contentType, itm);
            _entitiesController.Delete(contentType, id);
        }

	    private string Delete_SharedCode(string contentType, IEntity itm)
	    {
	        var autoAllowAdmin = true;
	        // special case: contentType "any" - in this case it looks up the type
	        if (contentType == "any")
	        {
	            autoAllowAdmin = false;
	            contentType = itm?.Type.Name;
	        }

	        // itm = _entitiesController.GetEntityOrThrowError(contentType, itm.EntityId, appId: App.AppId);
	        PerformSecurityCheck(contentType, PermissionGrant.Delete, autoAllowAdmin, itm);
	        return contentType;
	    }

	    [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public void Delete(string contentType, Guid guid)
        {
            InitEavAndSerializer();
            IEntity itm = _entitiesController.GetEntityOrThrowError(contentType, guid, App.AppId);

            //IEntity itm = App.Data.Out["Default"].LightList     // pre-fetch for security and content-type check
            //    .FirstOrDefault(e => e.EntityGuid == guid);
            contentType = Delete_SharedCode(contentType, itm);
            _entitiesController.Delete(contentType, guid);
        }

        #endregion

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
	    public Dictionary<string, object> GetOne(string contentType, int id, string appPath = null)
	    {
            // if app-path specified, use that app, otherwise use from context
            var appId = GetAppIdFromPathOrContext(appPath);

            InitEavAndSerializer(appId);
            IEntity itm = _entitiesController.GetEntityOrThrowError(contentType, id, appId);
            PerformSecurityCheck(contentType, PermissionGrant.Read, true, itm, useContext: appPath == null, appId: appId);
	        return _entitiesController.GetOne(contentType, id);
	    }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public Dictionary<string, object> GetOne(string contentType, Guid guid, string appPath = null)
        {
            // if app-path specified, use that app, otherwise use from context
            var appId = GetAppIdFromPathOrContext(appPath);

            InitEavAndSerializer(appId);
            IEntity itm = _entitiesController.GetEntityOrThrowError(contentType, guid, appId);
            PerformSecurityCheck(contentType, PermissionGrant.Read, true, itm, useContext: appPath == null, appId: appId);
            return _entitiesController.Serializer.Prepare(itm);
        }
        [HttpPost]
        [HttpPatch]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
	    public Dictionary<string, object> Create(string contentType, Dictionary<string, object> newContentItem)
	    {
            InitEavAndSerializer();
            // Now check standard create-permissions
            PerformSecurityCheck(contentType, PermissionGrant.Create, true);

            // Convert to case-insensitive dictionary just to be safe!
            newContentItem = new Dictionary<string, object>(newContentItem, StringComparer.OrdinalIgnoreCase);

            // Now create the cleaned up import-dictionary so we can create a new entity
            var cleanedNewItem = CreateEntityDictionary(contentType, newContentItem);

	        // Get user name/id when creating, would be important if one day checking for author
	        //var x = Dnn.User.UserID;
	        //var userName = (x == -1) ? "Anonymous" : "dnn:id=" + x;

	        var userName = Environment.Dnn7.UserIdentity.CurrentUserIdentityToken;

            // try to create
            App.Data.Create(contentType, cleanedNewItem, userName); // full version, with "who did it" for the log entry

            // Todo: try to return the newly created object 
            return null;
	    }


        /// <summary>
        /// Construct an import-friedly, type-controlled value-dictionary to create or update an entity
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="newContentItem"></param>
        /// <returns></returns>
	    private Dictionary<string, object> CreateEntityDictionary(string contentType, Dictionary<string, object> newContentItem)
	    {
            // Retrieve content-type definition and check all the fields that this content-type has
	        var cache = (BaseCache) DataSource.GetCache(App.ZoneId, App.AppId);
	        var listOfTypes = cache.GetContentType(contentType);// as ContentType;
	        var attribs = listOfTypes.AttributeDefinitions;


	        var cleanedNewItem = new Dictionary<string, object>();
	        foreach (var attrDef in attribs.Select(a => a.Value))
	        {
	            // var attrDef = attr.Value;// ats.GetAttribute(attr);
	            var attrName = attrDef.Name;
	            if (newContentItem.ContainsKey(attrName))
	            {
	                var foundValue = newContentItem[attrName];
	                switch (attrDef.Type.ToLower())
	                {
	                    case "string":
	                    case "hyperlink":
	                        if (foundValue is string)
	                            cleanedNewItem.Add(attrName, foundValue.ToString());
	                        else
	                            ThrowValueMappingError(attrDef, foundValue);
	                        break;
	                    case "boolean":
	                        bool bolValue;
	                        if (bool.TryParse(foundValue.ToString(), out bolValue))
	                            cleanedNewItem.Add(attrName, bolValue);
	                        else
	                            ThrowValueMappingError(attrDef, foundValue);
	                        break;
	                    case "datetime":
	                        DateTime dtm;
	                        if (DateTime.TryParse(foundValue.ToString(), out dtm))
	                            cleanedNewItem.Add(attrName, dtm);
	                        else
	                            ThrowValueMappingError(attrDef, foundValue);
	                        break;
	                    case "number":
	                        decimal dec;
	                        if (Decimal.TryParse(foundValue.ToString(), out dec))
	                            cleanedNewItem.Add(attrName, dec);
	                        else
	                            ThrowValueMappingError(attrDef, foundValue);
	                        break;
	                    case "entity":
	                        var relationships = new List<int>();

	                        var foundEnum = foundValue as System.Collections.IEnumerable;
	                        if (foundEnum != null) // it's a list!
	                            foreach (var item in foundEnum)
                                    relationships.Add(CreateSingleRelationshipItem(item));
	                        else // not a list
                                relationships.Add(CreateSingleRelationshipItem(foundValue));

	                        cleanedNewItem.Add(attrName, relationships);

	                        break;
	                    default:
	                        throw new Exception("Tried to create attribute '" + attrName + "' but the type is not known: '" +
	                                            attrDef.Type + "'");
	                }
	            }

	            // todo: maybe one day get default-values and insert them if not supplied by JS
	        }
	        return cleanedNewItem;
	    }

        /// <summary>
        /// In case of an error, show a nicer, consistent message
        /// </summary>
        /// <param name="attributeDefinition"></param>
        /// <param name="foundValue"></param>
	    private static void ThrowValueMappingError(IAttributeBase attributeDefinition, object foundValue)
	    {
	        throw new Exception("Tried to create " + attributeDefinition.Name + " and couldn't convert to correct " + attributeDefinition.Type + ": '" +
	                            foundValue + "'");
	    }

	    /// <summary>
	    /// Takes input from JSON which could be in many formats like Category=ID or Category={id=#} 
	    /// and then converts it to an item in the relationships-list
	    /// </summary>
	    /// <param name="foundValue"></param>
	    private int CreateSingleRelationshipItem(object foundValue)
	    {
	        try
	        {
	            // the object foundNumber is either just an Id, or an Id/Title combination
                // Try to see if it's already a number, else check if it's a JSON property
                int foundNumber;
	            if (!int.TryParse(foundValue.ToString(), out foundNumber))
	            {
	                JProperty jp = foundValue as JProperty;
                    if(jp != null)
	                    foundNumber = (int) (foundValue as JProperty).Value;
                    else
                    {
                        JObject jo = foundValue as JObject;
                        JToken foundId;
                        // ReSharper disable once PossibleNullReferenceException
                        if (jo.TryGetValue("Id", out foundId))
                            foundNumber = (int) foundId;
                        else if (jo.TryGetValue("id", out foundId))
                            foundNumber = (int) foundId;
                    }
	            }
	            return foundNumber;
	        }
	        catch
	        {
                throw new Exception("Tried to find Id of a relationship - but only found " + foundValue);
	        }
	    }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public Dictionary<string, object> Create(string contentType, int id, Dictionary<string, object> newContentItem)
        {
            InitEavAndSerializer();
            // Now check standard create-permissions

            // Check that this ID is actually of this content-type,
            // this throws an error if it's not the correct type
            IEntity itm = _entitiesController.GetEntityOrThrowError(contentType, id, appId: App.AppId);


            PerformSecurityCheck(contentType, PermissionGrant.Update, true, itm);


            // Convert to case-insensitive dictionary just to be safe!
            newContentItem = new Dictionary<string, object>(newContentItem, StringComparer.OrdinalIgnoreCase);

            // Now create the cleaned up import-dictionary so we can create a new entity
            var cleanedNewItem = CreateEntityDictionary(contentType, newContentItem);

            // Get user name/id when creating, would be important if one day checking for author
            var x = Dnn.User.UserID;
            var userName = (x == -1) ? "Anonymous" : "dnn:id=" + x;

            // try to create
            App.Data.Update(id, cleanedNewItem, userName); // full version, with "who did it" for the log entry

            // Todo: try to return the newly created object 
            return _entitiesController.Serializer.Prepare(App.Data.List[id]);
        }
    }
}
