using System.Collections.Generic;
using ToSic.Eav.Api.Api01;

namespace ToSic.SexyContent.DataSources
{
    public class App: Eav.DataSources.App
    {
        internal string DefaultLanguage { get; set; }
        internal string CurrentUserName { get; set; }

        /// <summary>
        /// Get a correctly instantiated instance of the simple data controller.
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>An data controller to create, update and delete entities</returns>
        private SimpleDataController DataController(string userName = null)
        {
            if (userName == null)
                userName = CurrentUserName;
            return new SimpleDataController(ZoneId, AppId, userName, DefaultLanguage);
        }

        public void Create(string contentTypeName,
            Dictionary<string, object> values, string userName = null)
        {
            var x = DataController(userName);
            x.Create(contentTypeName, values);
        }

        public void Update(int entityId, Dictionary<string, object> values,
            string userName = null)
        {
            var x = DataController(userName);
            x.Update(entityId, values);
        }


        public void Delete(int entityId, string userName = null)
        {
            var x = DataController(userName);
            x.Delete(entityId);
        }

 
    }
}