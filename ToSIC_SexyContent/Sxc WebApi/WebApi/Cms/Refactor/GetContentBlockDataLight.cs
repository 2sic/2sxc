using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.DataSources;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Compatibility;

namespace ToSic.Sxc.WebApi.Cms.Refactor
{
    internal class GetContentBlockDataLight
    {
        private readonly IBlockBuilder _blockBuilder;
        // todo i18n in the client - probably just use a code, and use the json translation
        private string errorText =
            "A module (content-block) is trying to retrieve data from the server as JSON. If you see this message, it is because Data Publishing is not enabled on the appropriate view. Please enable it in the view settings. \\nThis is happening on the module {0}.";
        public GetContentBlockDataLight(IBlockBuilder blockBuilder)
        {
            _blockBuilder = blockBuilder;
        }

        internal string GeneratePleaseEnableDataError(int instanceId)
            => "2sxc Content (" + instanceId + "): " + string.Format(errorText, instanceId);

        /// <summary>
        /// Returns a JSON string for the elements
        /// </summary>
        internal string GetJsonFromStreams(IDataSource source, string[] streamsToPublish)
        {
#pragma warning disable 612
            var ser = new OldContentBlockJsonSerialization(_blockBuilder.UserMayEdit);
#pragma warning restore 612

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