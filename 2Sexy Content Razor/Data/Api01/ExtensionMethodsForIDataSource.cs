using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using ToSic.Eav;
using ToSic.SexyContent.Data.Api01;

namespace ToSic.SexyContent.Razor.Data.Api01
{
    /// <summary>
    /// Place the data extension methods here for the razor-views.
    /// It is important to keep this in an own namespace, to allow us to switch to a 
    /// different API in the future without breaking the old one.
    /// </summary>
    public static class ExtensionMethodsForIDataSource
    {
        /// <summary>
        /// Get a correctly instantiated instance of the simple data controller.
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>An data controller to create, update and delete entities</returns>
        private static SimpleDataController DataController(this ToSic.Eav.DataSources.IDataSource ds, string userName = "")
        {
            //try
            //{
            //    var typedApp = ds as ToSic.Eav.DataSources.App;
            //}
            //catch 
            //{
                
            //}
            var defaultLanguage = "";
            //var languagesActive = SexyContent.GetCulturesWithActiveState(page.Dnn.Portal.PortalId, page.App.ZoneId).Any(c => c.Active);
            //if (languagesActive)
            //{
            //    defaultLanguage = page.Dnn.Portal.DefaultLanguage;
            //}
            return new SimpleDataController(ds.ZoneId, ds.AppId, "", defaultLanguage);
        }

        public static void Create(this ToSic.Eav.DataSources.IDataSource ds, string contentTypeName,
            Dictionary<string, object> values, string userName = "")
        {
            var x = ds.DataController(userName);
            x.Create(contentTypeName, values);
        }

        public static void Update(this ToSic.Eav.DataSources.IDataSource ds, int entityId, Dictionary<string, object> values,
            string userName = "")
        {
            var x = ds.DataController(userName);
            x.Update(entityId, values);
        }


        public static void Delete(this ToSic.Eav.DataSources.IDataSource ds, int entityId, string userName = "")
        {
            var x = ds.DataController(userName);
            x.Delete(entityId);
        }

    }
}