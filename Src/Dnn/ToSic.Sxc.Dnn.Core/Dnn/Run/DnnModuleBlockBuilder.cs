﻿using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnModuleBlockBuilder: HasLog<DnnModuleBlockBuilder>
    {
        public DnnModuleBlockBuilder(IContextOfBlock context, BlockFromModule block) : base($"{DnnConstants.LogName}.CtxBlF")
        {
            Context = context;
            _block = block;
        }
        protected readonly IContextOfBlock Context;
        private readonly BlockFromModule _block;
        protected bool AlreadyConfigured = false;
        private ILog _parentLog;

        public new DnnModuleBlockBuilder Init(ILog parentLog)
        {
            _parentLog = parentLog;
            return base.Init(parentLog);
        }


        public BlockFromModule GetBlockOfModule(ModuleInfo dnnModule)
        {
            var wrapLog = Log.Call<BlockFromModule>($"{dnnModule?.ModuleID}");
            if (dnnModule == null) throw new ArgumentNullException(nameof(dnnModule));
            if (AlreadyConfigured) throw new Exception($"{nameof(GetBlockOfModule)} can only be called once. Then you need a new service.");

            var initializedCtx = InitDnnSiteModuleAndBlockContext(Context, dnnModule);

            AlreadyConfigured = true;

            var result = _block.Init(initializedCtx, _parentLog);
            return wrapLog("ok", result);

        }

        private IContextOfBlock InitDnnSiteModuleAndBlockContext(IContextOfBlock context, ModuleInfo dnnModule)
        {
            context.Init(_parentLog);
            var wrapLog = Log.Call<IContextOfBlock>();
            Log.Add($"Will try-swap module info of {dnnModule.ModuleID} into site");
            ((DnnSite)context.Site).TrySwap(dnnModule, _parentLog);
            Log.Add("Will init module");
            ((DnnModule)context.Module).Init(dnnModule, _parentLog);
            return wrapLog(null, InitPageOnly(context));
        }

        private IContextOfBlock InitPageOnly(IContextOfBlock context)
        {
            var log = context.Log;
            var wrapLog = log.Call<IContextOfBlock>();
            // Collect / assemble page information
            var activeTab = (context.Site as Site<PortalSettings>)?.UnwrappedContents?.ActiveTab;
            context.Page.Init(activeTab?.TabID ?? Eav.Constants.NullId);

            // the FullUrl will throw an error in DNN search scenarios
            string url = null;
            try
            {
                // skip during search (usual HttpContext is missing for search)
                if (System.Web.HttpContext.Current == null) return wrapLog("no http-context, can't add page", context);

                url = activeTab?.FullUrl.TrimLastSlash();
                ((Page)context.Page).Url = url;
            }
            catch
            {
                /* ignore */
            }

            return wrapLog(url, context);
        }
    }
}