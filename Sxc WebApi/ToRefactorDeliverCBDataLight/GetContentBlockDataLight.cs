using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.Serializers;

namespace ToSic.SexyContent.WebApi.ToRefactorDeliverCBDataLight
{
    internal class GetContentBlockDataLight
    {
        private readonly SxcInstance _sxci;
        // todo i18n in the client - probably just use a code, and use the json translation
        private string errorText =
            "A module (contet-block) is trying to retrieve data from the server as JSON. If you see this message, it is because Data Publishing is not enabled on the appropriate view. Please enable it in the view settings. \\nThis is happening on the module {0}.";
        public GetContentBlockDataLight(SxcInstance sxc)
        {
            _sxci = sxc;
        }

        internal string GeneratePleaseEnableDataError(int instanceId)
            => "2sxc Content (" + instanceId + "): " + string.Format(errorText, instanceId);

        /// <summary>
        /// Returns a JSON string for the elements
        /// </summary>
        internal string GetJsonFromStreams(IDataSource source, string[] streamsToPublish)
        {
            var ser = new Serializer(_sxci);

            var y = streamsToPublish
                .Where(k => source.Out.ContainsKey(k))
                .ToDictionary(k => k, s => new
                {
                    List = (from c in source.Out[s].List select ser.PrepareOldFormat(c)).ToList()
                });

            return JsonConvert.SerializeObject(y);
        }


    }
}