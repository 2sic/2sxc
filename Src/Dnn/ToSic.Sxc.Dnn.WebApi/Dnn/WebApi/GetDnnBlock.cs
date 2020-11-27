using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Web.Api;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Run;

using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.WebApi
{
    internal class DnnGetBlock
    {
        private readonly IServiceProvider _serviceProvider;

        public DnnGetBlock(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        internal IBlock GetCmsBlock(HttpRequestMessage request, bool allowNoContextFound, ILog log)
        {
            var wrapLog = log.Call<IBlock>($"request:..., {nameof(allowNoContextFound)}: {allowNoContextFound}");

            var moduleInfo = request.FindModuleInfo();

            if (allowNoContextFound & moduleInfo == null)
                return wrapLog("request ModuleInfo not found, allowed", null);
            
            if (moduleInfo == null)
                log.Add("context/module not found");

            var context = _serviceProvider.Build<IContextOfBlock>().Init(moduleInfo, log);
            context.Page.Parameters = GetOverrideParams(request);
            IBlock block = _serviceProvider.Build<BlockFromModule>().Init(context, log);

            // check if we need an inner block
            if (request.Headers.Contains(WebApiConstants.HeaderContentBlockId)) { 
                var blockHeaderId = request.Headers.GetValues(WebApiConstants.HeaderContentBlockId).FirstOrDefault();
                int.TryParse(blockHeaderId, out var blockId);
                if (blockId < 0)   // negative id, so it's an inner block
                {
                    log.Add($"Inner Content: {blockId}");
                    block = _serviceProvider.Build<BlockFromEntity>().Init(block, blockId, log);
                }
            }

            return wrapLog("ok", block);
        }

        /// <summary>
        /// get url parameters and provide override values to ensure all configuration is 
        /// preserved in AJAX calls
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static List<KeyValuePair<string, string>> GetOverrideParams(HttpRequestMessage request)
        {
            List<KeyValuePair<string, string>> urlParams = null;
            var requestParams = request.GetQueryNameValuePairs();
            var origParams = requestParams.Where(p => p.Key == "originalparameters").ToList();
            if (origParams.Any())
            {
                var paramSet = origParams.First().Value;

                // Workaround for deserializing KeyValuePair -it requires lowercase properties(case sensitive),
                // which seems to be a bug in some Newtonsoft.Json versions: http://stackoverflow.com/questions/11266695/json-net-case-insensitive-property-deserialization
                var items = Json.Deserialize<List<UpperCaseStringKeyValuePair>>(paramSet);
                urlParams = items.Select(a => new KeyValuePair<string, string>(a.Key, a.Value)).ToList();
            }

            return urlParams;
        }


        /// <summary>
        /// Workaround for deserializing KeyValuePair - it requires lowercase properties (case sensitive), 
        /// which seems to be a issue in some Newtonsoft.Json versions: http://stackoverflow.com/questions/11266695/json-net-case-insensitive-property-deserialization
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Local
        private class UpperCaseStringKeyValuePair
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Key { get; set; }
            public string Value { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

    }


}
