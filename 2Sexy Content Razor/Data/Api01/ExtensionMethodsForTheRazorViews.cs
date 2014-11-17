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
            var defaultLanguage = "";
            var languagesActive = SexyContent.GetCulturesWithActiveState(page.Dnn.Portal.PortalId, page.App.ZoneId).Any(c => c.Active);
            if (languagesActive)
            {
                defaultLanguage = page.Dnn.Portal.DefaultLanguage;
            }
            return new SimpleDataController(page.App.ZoneId, page.App.AppId, page.User.Identity.Name, defaultLanguage);
        }
    }
}