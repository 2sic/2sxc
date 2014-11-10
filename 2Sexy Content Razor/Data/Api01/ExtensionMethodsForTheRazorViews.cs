using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using ToSic.SexyContent.Data.Api01;

namespace ToSic.SexyContent.Razor.Data.Api01
{
    /// <summary>
    /// Place the data extension methods here for the razor-views.
    /// It is important to keep this in an own namespace, to allow us to switch to a 
    /// different API in the future without breaking the old one.
    /// </summary>
    public static class ExtensionMethodsForTheRazorViews
    {
        /// <summary>
        /// Get a correctly instantiated instance of the simple data controller.
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>An data controller to create, update and delete entities</returns>
        public static SimpleDataController DataController(this SexyContentWebPage page)
        {
            return new SimpleDataController(page.App.ZoneId, page.App.AppId, page.User.Identity.Name, page.Dnn.Portal.DefaultLanguage);
        }

        /// <summary>
        /// Create a new entity of the content-type specified.
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="contentTypeName">Content-type</param>
        /// <param name="values">Values to be set (dictionary of attribute name and value pairs)</param>
        /// <returns>True if create succeeded, else false</returns>
        public static bool CreateEntity(this SexyContentWebPage page, string contentTypeName, Dictionary<string, object> values)
        {
            return page.DataController().Create(contentTypeName, values);
        }

        /// <summary>
        /// Update the entity specified by ID.
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="entityId">Entity ID</param>
        /// <param name="values">Values to be updated (dictionary of attribute name and value pairs)</param>
        /// <returns>True if update succeeded, else false</returns>
        public static bool UpdateEntity(this SexyContentWebPage page, int entityId, Dictionary<string, object> values)
        {
            return page.DataController().Update(entityId, values);
        }

        /// <summary>
        /// Update the entity specified by Guid.
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="entityGuid">Entity Guid</param>
        /// <param name="values">Values to be updated (dictionary of attribute name and value pairs)</param>
        /// <returns>True if update succeeded, else false</returns>
        public static bool UpdateEntity(this SexyContentWebPage page, Guid entityGuid, Dictionary<string, object> values)
        {
            return page.DataController().Update(entityGuid, values);
        }

        /// <summary>
        /// Delete the entity specified by ID.
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="entityId">Entity ID</param>
        /// <returns>True if delete succeeded, else false</returns>
        public static bool DeleteEntity(this SexyContentWebPage page, int entityId)
        {
            return page.DataController().Delete(entityId);
        }

        /// <summary>
        /// Delete the entity specified by Guid.
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="entityGuid">Entity Guid</param>
        /// <returns>True if delete succeeded, else false</returns>
        public static bool DeleteEntity(this SexyContentWebPage page, Guid entityGuid)
        {
            return page.DataController().Delete(entityGuid);
        }
    }

}