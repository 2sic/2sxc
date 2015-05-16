using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Telerik.Web.UI.PivotGrid.Core.Fields;
using ToSic.Eav;
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
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public void Delete(string contentType, int id)
        {
            InitEavAndSerializer();
             _eavWebApi.Delete(contentType, id);
        }
        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void Delete(string contentType, Guid guid)
        {
            InitEavAndSerializer();
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
            
            // todo: maybe perform extra "does this stuff look right" checks to deliver nicer errors

            // todo: maybe one day get default-values and insert them if not supplied by JS

            // todo: get user name/id when creating, would be important if one day checking for author
            // ...
	        var x = Dnn.User.Username;
	        var userName = "Anonymous";

            // try to create
            App.Data.Create(contentType, newContentItem, userName); // full version, with "who did it" for the log entry

            return null;
	    }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public Dictionary<string, object> CreateOrUpdate(string contentType, int id)
        {
            return null;
        }
    }
}
