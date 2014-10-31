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
    /// It's important to keep this in an own namespace, to allow us to switch to a different API in the future without breaking the old one.
    /// </summary>
    public static class ExtensionMethodsForTheRazorViews
    {
        /// <summary>
        /// this will simply return a correctly instantiated instance of the simple data controller.
        /// </summary>
        /// <param name="page"></param>
        /// <returns>A instantiaced DataController which can Create/Update/Delete entities</returns>
        public static SimpleDataController DataController(this SexyContentWebPage page)
        {
            // todo: ensure I have the language & appid
            return new SimpleDataController(page.App.AppId, "");
        }


        public static bool Create(this SexyContentWebPage page, string contentTypeName, Dictionary<string, object> values)
        {
            return page.DataController().Create(contentTypeName, values);
        }
        public static bool Update(this SexyContentWebPage page, int entityId, Dictionary<string, object> values)
        {
            return page.DataController().Update(entityId, values);
        }
        public static bool Create(this SexyContentWebPage page, int entityId)
        {
            return page.DataController().Delete(entityId);
        }
    }

}