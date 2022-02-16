using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn
{
    public class DnnModuleAndBlockBuilder: ModuleAndBlockBuilder
    {
        public DnnModuleAndBlockBuilder(Generator<IModule> moduleGenerator, Generator<IContextOfBlock> contextGenerator, Generator<BlockFromModule> blockGenerator) : base(DnnConstants.LogName)
        {
            _moduleGenerator = moduleGenerator;
            _contextGenerator = contextGenerator;
            _blockGenerator = blockGenerator;
        }
        private readonly Generator<IModule> _moduleGenerator;
        private readonly Generator<IContextOfBlock> _contextGenerator;
        private readonly Generator<BlockFromModule> _blockGenerator;
        private ILog ParentLog => Log.Parent ?? Log;


        protected override IModule GetModuleImplementation(int pageId, int moduleId)
        {
            var moduleInfo = new ModuleController().GetModule(moduleId, pageId, false);
            ThrowIfModuleIsNull(pageId, moduleId, moduleInfo);
            var module = ((DnnModule)_moduleGenerator.New).Init(moduleInfo, Log);
            return module;
        }
        protected override IBlock GetBlock(IModule module) => GetBlock((module as DnnModule)?.GetContents());


        public override IBlock GetBlock<TPlatformModule>(TPlatformModule module)
        {
            var wrapLog = Log.Call<BlockFromModule>();
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (!(module is ModuleInfo dnnModule)) throw new ArgumentException("Given data is not a module");
            Log.Add($"Module: {dnnModule.ModuleID}");

            var initializedCtx = InitDnnSiteModuleAndBlockContext(dnnModule);
            var result = _blockGenerator.New.Init(initializedCtx, ParentLog);
            return wrapLog("ok", result);
        }

        private IContextOfBlock InitDnnSiteModuleAndBlockContext(ModuleInfo dnnModule)
        {
            var wrapLog = Log.Call<IContextOfBlock>();
            var context = _contextGenerator.New;
            context.Init(ParentLog);
            Log.Add($"Will try-swap module info of {dnnModule.ModuleID} into site");
            ((DnnSite)context.Site).TrySwap(dnnModule, ParentLog);
            Log.Add("Will init module");
            ((DnnModule)context.Module).Init(dnnModule, ParentLog);
            return wrapLog(null, InitPageOnly(context));
        }

        private IContextOfBlock InitPageOnly(IContextOfBlock context)
        {
            var wrapLog = Log.Call<IContextOfBlock>();
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
