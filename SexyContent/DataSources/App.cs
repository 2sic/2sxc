using System.Collections.Generic;
using ToSic.Eav.Api.Api01;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.DataSources
{
    public class App: Eav.DataSources.App, IAppData
    {
        public App(Log parentLog = null)
        {
            InitLog("EaApDS", parentLog);
        }

        internal string DefaultLanguage { get; set; }
        internal string CurrentUserName { get; set; }

        /// <summary>
        /// Get a correctly instantiated instance of the simple data controller.
        /// </summary>
        /// <returns>An data controller to create, update and delete entities</returns>
        private SimpleDataController DataController() => new SimpleDataController(ZoneId, AppId, DefaultLanguage, Log);

        public void Create(string contentTypeName,
            Dictionary<string, object> values, string userName = null)
        {
            Log.Add($"app create new entity of type:{contentTypeName}");
            var x = DataController();
            x.Create(contentTypeName, values);
        }

        public void Update(int entityId, Dictionary<string, object> values,
            string userName = null)
        {
            Log.Add($"app update i:{entityId}");
            var x = DataController();
            x.Update(entityId, values);
        }


        public void Delete(int entityId, string userName = null)
        {
            Log.Add($"app delete i:{entityId}");
            var x = DataController();
            x.Delete(entityId);
        }
 
    }
}