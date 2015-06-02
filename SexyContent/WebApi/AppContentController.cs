using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources.Caches;
using ToSic.SexyContent.Security;
using ToSic.SexyContent.Serializers;

namespace ToSic.SexyContent.WebApi
{
	/// <summary>
	/// Direct access to app-content items, simple manipulations etc.
	/// Should check for security at each standard call - to see if the current user may do this
	/// Then we can reduce security access level to anonymous, because each method will do the security check
	/// todo: security
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	// [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
	public class AppContentController : SxcApiController
	{
	    private Eav.WebApi.WebApi _eavWebApi;

		public AppContentController()
		{
			Eav.Configuration.SetConnectionString("SiteSqlServer");
            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
            //		    (eavWebApi.Serializer as Serializer).Sxc = Sexy;
		}

	    private void InitEavAndSerializer()
	    {
            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
            _eavWebApi = new Eav.WebApi.WebApi(App.AppId);
            ((Serializer)_eavWebApi.Serializer).Sxc = Sexy;	        
	    }

        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, string cultureCode = null)
        {
            InitEavAndSerializer();
            PerformSecurityCheck(contentType, PermissionGrant.Read, true);
            return _eavWebApi.GetEntities(contentType, cultureCode);
        }

        /// <summary>
        /// Check if a user may do something - and throw an error if the permission is not given
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="grant"></param>
	    private void PerformSecurityCheck(string contentType, PermissionGrant grant, bool autoAllowAdmin = false)
	    {
            // Check if we can find this content-type
            var ct = _eavWebApi.GetContentType(contentType, App.AppId);
            if(ct == null)
                ThrowHttpError(HttpStatusCode.NotFound, "Could not find Content Type '" + contentType + "'.", "content-types");
            
            // Check if the content-type has a GUID as name - only these can have permission assignments
            Guid ctGuid;
	        var staticNameIsGuid = Guid.TryParse(ct.StaticName, out ctGuid);
            if(!staticNameIsGuid)
                ThrowHttpError(HttpStatusCode.Unauthorized, "Content Type '" + contentType + "' is not a standard Content Type - no permissions possible.");

            // Check permissions in 2sxc - or check if the user has admin-right (in which case he's always granted access for these types of content)
            var permissionChecker = new PermissionController(App.ZoneId, App.AppId, ctGuid, Dnn.Module);
            var allowed = permissionChecker.UserMay(grant);

            var isAdmin = autoAllowAdmin && DotNetNuke.Security.Permissions.ModulePermissionController.CanAdminModule(Dnn.Module);

            if(!(allowed || isAdmin))
                ThrowHttpError(HttpStatusCode.Unauthorized, "Request not allowed. User needs permissions to " + grant + " for Content Type '" + contentType + "'.", "permissions"); 
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
		    return _eavWebApi.GetAssignedEntities(assignmentObjectTypeId, keyGuid, contentType);
		}

        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
	    public void Delete(string contentType, int id)
        {
            InitEavAndSerializer();
            PerformSecurityCheck(contentType, PermissionGrant.Delete, true);
            _eavWebApi.Delete(contentType, id);
        }
        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public void Delete(string contentType, Guid guid)
        {
            InitEavAndSerializer();
            PerformSecurityCheck(contentType, PermissionGrant.Delete, true);
            _eavWebApi.Delete(contentType, guid);
        }


	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
	    public Dictionary<string, object> GetOne(string contentType, int id)
	    {
            InitEavAndSerializer();
            PerformSecurityCheck(contentType, PermissionGrant.Read, true);
	        return _eavWebApi.GetOne(contentType, id);
	    }

	    [HttpPost]
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
	        var x = Dnn.User.UserID;
	        var userName = (x == -1) ? "Anonymous" : "dnn:id=" + x;

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
	        var listOfTypes = cache.GetContentType(contentType) as ContentType;
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
	    private static void ThrowValueMappingError(AttributeBase attributeDefinition, object foundValue)
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
            PerformSecurityCheck(contentType, PermissionGrant.Update, true);

            // Check that this ID is actually of this content-type, this throws an error if it's not the correct type
            _eavWebApi.GetOne(contentType, id);

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
            return _eavWebApi.Serializer.Prepare(App.Data.List[id]);
        }
    }
}
