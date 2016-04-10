using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.WebApi.ToRefactorDeliverCBDataLight
{
    internal class GetContentBlockDataLight
    {
        private SxcInstance _sxci;
        // todo 8.4 i18n in the client
        private string errorText =
            "A module (contet-block) is trying to retrieve data from the server as JSON. If you see this message, it is because Data Publishing is not enabled on the appropriate view. Please enable it in the view settings. \\nThis is happening on the module {0} (the module with the title \"{1}\").";
        public GetContentBlockDataLight(SxcInstance sxc)
        {
            _sxci = sxc;
        }

        internal string GeneratePleaseEnableDataError(int instanceId, string moduleTitle)
            => "2sxc Content (" + instanceId + "): " + string.Format(errorText, instanceId, moduleTitle);

        /// <summary>
        /// Returns a JSON string for the elements
        /// </summary>
        public string GetJsonFromStreams(IDataSource source, string[] streamsToPublish)
        {
            var language = Thread.CurrentThread.CurrentCulture.Name;

            var y = streamsToPublish.Where(k => source.Out.ContainsKey(k)).ToDictionary(k => k, s => new
            {
                List = (from c in source.Out[s].List select new DynamicEntity(c.Value, new[] { language }, _sxci).ToDictionary() /*Sexy.ToDictionary(c.Value, language)*/).ToList()
            });

            return JsonConvert.SerializeObject(y);
        }


    }
}