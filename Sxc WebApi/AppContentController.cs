using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using ToSic.Eav;
using ToSic.Eav.Data.Query;
using ToSic.Eav.DataSources.Caches;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Serializers;
using ToSic.SexyContent.WebApi.ToRefactorDeliverCBDataLight;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [AllowAnonymous]
    public class AppContentController : SxcApiController
	{
	    private EntitiesController _entitiesController;

	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
	        Log.Rename("2sApCo");
	    }

        #region Get List / all of a certain content-type
        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, string appPath = null, string cultureCode = null)
        {
            Log.Add($"get entities type:{contentType}, path:{appPath}, culture:{cultureCode}");
            // if app-path specified, use that app, otherwise use from context
            var appId = GetAppIdFromPathOrContext_AndInitEavAndSerializer(appPath);

            PerformSecurityCheck(contentType, PermissionGrant.Read, true, useContext: appPath == null, appId: appId);
            return _entitiesController.GetEntities(contentType, cultureCode);
        }

        #endregion


        #region GetOne by ID / GUID

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public Dictionary<string, object> GetOne(string contentType, int id, string appPath = null)
            => GetAndSerializeOneAfterSecurityChecks(contentType,
                appId => _entitiesController.GetEntityOrThrowError(contentType, id/*, appId*/),
                appPath);


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public Dictionary<string, object> GetOne(string contentType, Guid guid, string appPath = null)
            => GetAndSerializeOneAfterSecurityChecks(contentType,
                appId => _entitiesController.GetEntityOrThrowError(contentType, guid, appId),
                appPath);
        


        /// <summary>
        /// Preprocess security / context, then get the item based on an passed in method, 
        /// ...then process/finish
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="getOne"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetAndSerializeOneAfterSecurityChecks(string contentType, Func<int, IEntity> getOne, string appPath)
        {
            Log.Add($"get and serialie after security check type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appId = GetAppIdFromPathOrContext_AndInitEavAndSerializer(appPath);

            IEntity itm = getOne(appId);
            PerformSecurityCheck(contentType, PermissionGrant.Read, true, itm, useContext: appPath == null, appId: appId);
            return _entitiesController.Serializer.Prepare(itm);
        }

        #endregion

        #region ContentBlock - retrieving data of the current instance as is
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
	    public HttpResponseMessage GetContentBlockData()
        {
            Log.Add("get content block data");
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
                    {ReasonPhrase = dataHandler.GeneratePleaseEnableDataError(SxcContext.InstanceInfo.Id)});
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }
        #endregion



        #region Create
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public Dictionary<string, object> CreateOrUpdate([FromUri] string contentType, [FromBody] Dictionary<string, object> newContentItem, [FromUri] int? id = null, [FromUri] string appPath = null)
        {
            Log.Add($"create or update type:{contentType}, id:{id}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appId = GetAppIdFromPathOrContext_AndInitEavAndSerializer(appPath);

            // Check that this ID is actually of this content-type,
            // this throws an error if it's not the correct type
            var itm = id == null 
                ? null 
                : _entitiesController.GetEntityOrThrowError(contentType, id.Value);

            var perm = id == null 
                ? PermissionGrant.Create 
                : PermissionGrant.Update;

            PerformSecurityCheck(contentType, perm, true, itm, appPath == null, appId);

            // Convert to case-insensitive dictionary just to be safe!
            newContentItem = new Dictionary<string, object>(newContentItem, StringComparer.OrdinalIgnoreCase);

            // Now create the cleaned up import-dictionary so we can create a new entity
            var cleanedNewItem = CreateEntityDictionary(contentType, newContentItem, appId);

            var userName = Environment.Dnn7.UserIdentity.CurrentUserIdentityToken;

            // try to create
            var currentApp = new App(PortalSettings, appId);
            //currentApp.InitData(false, new ValueCollectionProvider());
            currentApp.InitData(false, 
                new Environment.Dnn7.PagePublishing(Log).IsEnabled(ActiveModule.ModuleID), 
                Data.ConfigurationProvider);
            if (id == null)
            {
                currentApp.Data.Create(contentType, cleanedNewItem, userName);
                // Todo: try to return the newly created object 
                return null;
            }
            else
            {
                currentApp.Data.Update(id.Value, cleanedNewItem, userName);
                return _entitiesController.Serializer.Prepare(currentApp.Data.List.One(id.Value));
            }
        }

        /// <summary>
        /// Construct an import-friedly, type-controlled value-dictionary to create or update an entity
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="newContentItem"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        private Dictionary<string, object> CreateEntityDictionary(string contentType, Dictionary<string, object> newContentItem, int appId)
        {
            Log.Add($"create ent dic a#{appId}, type:{contentType}");
            // Retrieve content-type definition and check all the fields that this content-type has
	        var cache = (BaseCache) DataSource.GetCache(null, appId);
	        var listOfTypes = cache.GetContentType(contentType);// as ContentType;
	        var attribs = listOfTypes.Attributes;


	        var cleanedNewItem = new Dictionary<string, object>();
	        foreach (var attrDef in attribs)
	        {
	            // var attrDef = attr.Value;// ats.GetAttribute(attr);
	            var attrName = attrDef.Name;
	            if (!newContentItem.ContainsKey(attrName)) continue;
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
	                    if (decimal.TryParse(foundValue.ToString(), out dec))
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
	        Log.Add("create relationship");
	        try
	        {
	            // the object foundNumber is either just an Id, or an Id/Title combination
                // Try to see if it's already a number, else check if it's a JSON property
	            if (!int.TryParse(foundValue.ToString(), out var foundNumber))
	            {
	                if(foundValue is JProperty jp)
	                    foundNumber = (int) jp.Value;
                    else
                    {
                        var jo = foundValue as JObject;
                        // ReSharper disable once PossibleNullReferenceException
                        if (jo.TryGetValue("Id", out var foundId))
                            foundNumber = (int) foundId;
                        else if (jo.TryGetValue("id", out foundId))
                            foundNumber = (int) foundId;
                    }
	            }
	            Log.Add($"relationship found:{foundNumber}");
	            return foundNumber;
	        }
	        catch
	        {
                throw new Exception("Tried to find Id of a relationship - but only found " + foundValue);
	        }
	    }



        #endregion



        #region Delete

        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public void Delete(string contentType, int id, [FromUri] string appPath = null)
        {
            Log.Add($"delete id:{id}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appId = GetAppIdFromPathOrContext_AndInitEavAndSerializer(appPath);

            // don't allow type "any" on this
            if (contentType == "any")
                throw new Exception("type any not allowed with id-only, requires guid");

            IEntity itm = _entitiesController.GetEntityOrThrowError(contentType, id/*, appId*/);
            //var autoAllowAdmin = contentType != "any"; // only auto-allow-admin, if a type is clearly specified (protection from deleting other types)
            //Delete_SharedCode(itm, appPath == null, appId, autoAllowAdmin);
            PerformSecurityCheck(itm.Type.Name, PermissionGrant.Delete, true, itm, appPath == null, appId);
            _entitiesController.Delete(itm.Type.Name, id, appId);
        }

	    //private void Delete_SharedCode(IEntity itm, bool useContext, int appId, bool autoAllowAdmin)
	    //{
	    //    PerformSecurityCheck(itm.Type.Name, PermissionGrant.Delete, true, itm, useContext, appId);
	    //}

	    [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public void Delete(string contentType, Guid guid, [FromUri] string appPath = null)
        {
            Log.Add($"delete guid:{guid}, type:{contentType}, path:{appPath}");
            // if app-path specified, use that app, otherwise use from context
            var appId = GetAppIdFromPathOrContext_AndInitEavAndSerializer(appPath);
	        IEntity itm = _entitiesController.GetEntityOrThrowError(contentType == "any" ? null : contentType, guid, appId);

            //var autoAllowAdmin = true; // with guid, admins are allowed to delete
            //   Delete_SharedCode(itm, appPath == null, appId, autoAllowAdmin);
            PerformSecurityCheck(itm.Type.Name, PermissionGrant.Delete, true, itm, appPath == null, appId);
            _entitiesController.Delete(itm.Type.Name, guid, appId);
        }

        #endregion

        #region GetAssigned - unclear if in use!
        /// <summary>
        /// Get Entities with specified AssignmentObjectTypeId and Key
        /// todo: unclear if this is in use anywhere? 
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
		public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int assignmentObjectTypeId, Guid keyGuid, string contentType, [FromUri] string appPath = null)
        {
            Log.Add($"get assigned for assigmentType#{assignmentObjectTypeId}, guid:{keyGuid}, type:{contentType}, path:{appPath}");
            InitEavAndSerializer();
	        return new MetadataController().GetAssignedEntities(assignmentObjectTypeId, "guid", keyGuid.ToString(), contentType);
		}
        #endregion


        #region helpers / initializers to prep the EAV and Serializer

	    private void InitEavAndSerializer(int? appId = null)
	    {
	        Log.Add($"init eav for a#{appId}");
            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
            _entitiesController = new EntitiesController(appId ?? App.AppId);

            // only do this if we have a real context - otherwise don't do this
	        if (!appId.HasValue)
	            ((Serializer) _entitiesController.Serializer).Sxc = SxcContext;

	    }

        /// <summary>
        /// Retrieve the appId - either based on the parameter, or if missing, use context
        /// Note that this will fail, if both appPath and context are missing
        /// </summary>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private int GetAppIdFromPathOrContext_AndInitEavAndSerializer(string appPath)
        {
            Log.Add($"auto detect app and init eav - path:{appPath}");
            var appId = appPath == null || appPath == "auto"
                ? App.AppId
                : GetCurrentAppIdFromPath(appPath);
            InitEavAndSerializer(appId);
            return appId;
        }

        #endregion

       

    }
}
