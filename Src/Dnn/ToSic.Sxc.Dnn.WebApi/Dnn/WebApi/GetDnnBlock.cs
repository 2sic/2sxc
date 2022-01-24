using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Web.Api;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Web.Parameters;
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

        internal IBlock GetCmsBlock(HttpRequestMessage request, ILog log)
        {
            var wrapLog = log.Call<IBlock>(useTimer: true);

            var moduleInfo = request.FindModuleInfo();

            if (moduleInfo == null)
                return wrapLog("request ModuleInfo not found", null);
            
            var context = _serviceProvider.Build<IContextOfBlock>().Init(moduleInfo, log);
            // WebAPI calls can contain the original parameters that made the page, so that views can respect that
            // Probably replace with OriginalParameters.GetOverrideParams(context.Page.Parameters);
            // once it has proven stable in Oqtane
            context.Page.ParametersInternalOld = GetOverrideParams(request);
            IBlock block = _serviceProvider.Build<BlockFromModule>().Init(context, log);

            // check if we need an inner block
            if (request.Headers.Contains(WebApiConstants.HeaderContentBlockId)) { 
                var blockHeaderId = request.Headers.GetValues(WebApiConstants.HeaderContentBlockId).FirstOrDefault();
                int.TryParse(blockHeaderId, out var blockId);
                if (blockId < 0)   // negative id, so it's an inner block
                {
                    log.Add($"Inner Content: {blockId}");
                    if (request.Headers.Contains("BlockIds"))
                    {
                        var blockIds = request.Headers.GetValues("BlockIds").FirstOrDefault()?.Split(',');
                        block = FindInnerContentParentBlock(block, blockId, blockIds, log);
                    }
                    block = _serviceProvider.Build<BlockFromEntity>().Init(block, blockId, log);
                }
            }

            return wrapLog("ok", block);
        }

        private IBlock FindInnerContentParentBlock(IBlock parent, int contentBlockId, string[] blockIds, ILog log)
        {
            if (blockIds != null && blockIds.Length >= 2)
            {
                foreach (var ids in blockIds) // blockIds is ordered list, from first ancestor till last successor 
                {
                    var parentIds = ids.Split(':');
                    //var parentAppId = int.Parse(parentIds[0]);
                    //var parentContentBlocks = new Guid(parentIds[1]);
                    var id = int.Parse(parentIds[0]);
                    if (!int.TryParse(parentIds[1], out var cbid) || id == cbid || cbid >= 0) continue;
                    if (cbid == contentBlockId) break; // we are done, because block should be parent/ancestor of cbid
                    parent = _serviceProvider.Build<BlockFromEntity>().Init(parent, cbid, log);
                }
            }

            return parent;
        }

        /// <summary>
        /// get url parameters and provide override values to ensure all configuration is 
        /// preserved in AJAX calls
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // TODO: this is probably old code, and should be merged centrally to use the generic implementation
        // and be the same for Dnn and Oqtane
        private static List<KeyValuePair<string, string>> GetOverrideParams(HttpRequestMessage request)
        {
            List<KeyValuePair<string, string>> urlParams = null;
            var requestParams = request.GetQueryNameValuePairs();
            var origParams = requestParams.Where(p => p.Key == OriginalParameters.NameInUrlForOriginalParameters).ToList();
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




    }


}
