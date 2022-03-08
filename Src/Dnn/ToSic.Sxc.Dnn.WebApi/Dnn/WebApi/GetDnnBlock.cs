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

            // WebAPI calls can contain the original parameters that made the page, so that views can respect that
            IBlock block = _serviceProvider.Build<IModuleAndBlockBuilder>().Init(log).GetBlock(moduleInfo);
            block.Context.Page.ParametersInternalOld = OriginalParameters.GetOverrideParams(request.GetQueryNameValuePairs().ToList());

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
    }
}
