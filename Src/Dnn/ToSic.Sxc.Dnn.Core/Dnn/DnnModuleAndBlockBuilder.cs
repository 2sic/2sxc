using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn
{
    public class DnnModuleAndBlockBuilder: ModuleAndBlockBuilder
    {
        public DnnModuleAndBlockBuilder(Generator<IModule> moduleGenerator, Generator<IContextOfBlock> contextGenerator, Generator<BlockFromModule> blockGenerator) : base(DnnConstants.LogName)
        {
            ConnectServices(
                _moduleGenerator = moduleGenerator,
                _contextGenerator = contextGenerator,
                _blockGenerator = blockGenerator
            );
        }
        private readonly Generator<IModule> _moduleGenerator;
        private readonly Generator<IContextOfBlock> _contextGenerator;
        private readonly Generator<BlockFromModule> _blockGenerator;
        private ILog ParentLog => (Log as Log)?.Parent ?? Log;


        protected override IModule GetModuleImplementation(int pageId, int moduleId)
        {
            var wrapLog = Log.Fn<IModule>($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}");
            var moduleInfo = new ModuleController().GetModule(moduleId, pageId, false);

            wrapLog.A($"Page Id on DNN Module: {moduleInfo.TabID} - should be {pageId}");

            ThrowIfModuleIsNull(pageId, moduleId, moduleInfo);
            var module = ((DnnModule)_moduleGenerator.New()).Init(moduleInfo, Log);
            wrapLog.A($"Page Id on IModule: {module.BlockIdentifier} - should be {pageId}");
            return wrapLog.Return(module);
        }
        protected override IBlock GetBlock(IModule module, int? pageId) => GetBlock((module as DnnModule)?.GetContents(), pageId);


        public override IBlock GetBlock<TPlatformModule>(TPlatformModule module, int? pageId)
        {
            var wrapLog = Log.Fn<BlockFromModule>(startTimer: true);
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (!(module is ModuleInfo dnnModule)) throw new ArgumentException("Given data is not a module");
            Log.A($"Module: {dnnModule.ModuleID}");

            var initializedCtx = InitDnnSiteModuleAndBlockContext(dnnModule, pageId);
            var result = _blockGenerator.New().Init(initializedCtx);
            return wrapLog.ReturnAsOk(result);
        }

        private IContextOfBlock InitDnnSiteModuleAndBlockContext(ModuleInfo dnnModule, int? pageId)
        {
            var wrapLog = Log.Fn<IContextOfBlock>();
            var context = _contextGenerator.New();
            context.Init(ParentLog);
            Log.A($"Will try-swap module info of {dnnModule.ModuleID} into site");
            ((DnnSite)context.Site).TrySwap(dnnModule, ParentLog);
            Log.A("Will init module");
            ((DnnModule)context.Module).Init(dnnModule, ParentLog);
            return wrapLog.Return(InitPageOnly(context, pageId));
        }

        private IContextOfBlock InitPageOnly(IContextOfBlock context, int? pageId)
        {
            var wrapLog = Log.Fn<IContextOfBlock>();
            // Collect / assemble page information
            var activeTab = (context.Site as Site<PortalSettings>)?.UnwrappedContents?.ActiveTab;

            var page = (DnnPage)context.Page;
            var url = page.InitPageIdAndUrl(activeTab, pageId);

            return wrapLog.Return(context, url);
        }

    }
}
