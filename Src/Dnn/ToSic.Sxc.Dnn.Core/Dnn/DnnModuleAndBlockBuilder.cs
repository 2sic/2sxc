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


        protected override IModule GetModuleImplementation(int pageId, int moduleId) => Log.Func($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}", l =>
        {
            var moduleInfo = new ModuleController().GetModule(moduleId, pageId, false);

            l.A($"Page Id on DNN Module: {moduleInfo.TabID} - should be {pageId}");

            ThrowIfModuleIsNull(pageId, moduleId, moduleInfo);
            var module = ((DnnModule)_moduleGenerator.New()).Init(moduleInfo);
            l.A($"Page Id on IModule: {module.BlockIdentifier} - should be {pageId}");
            return module;
        });
        protected override IBlock GetBlock(IModule module, int? pageId) => GetBlock((module as DnnModule)?.GetContents(), pageId);


        public override IBlock GetBlock<TPlatformModule>(TPlatformModule module, int? pageId) => Log.Func(timer: true, func: l =>
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (!(module is ModuleInfo dnnModule)) throw new ArgumentException("Given data is not a module");
            l.A($"Module: {dnnModule.ModuleID}");

            var initializedCtx = InitDnnSiteModuleAndBlockContext(dnnModule, pageId);
            var result = _blockGenerator.New().Init(initializedCtx);
            return result;
        });

        private IContextOfBlock InitDnnSiteModuleAndBlockContext(ModuleInfo dnnModule, int? pageId) => Log.Func(() =>
        {
            var context = _contextGenerator.New();
            Log.A($"Will try-swap module info of {dnnModule.ModuleID} into site");
            ((DnnSite)context.Site).TrySwap(dnnModule, ParentLog);
            Log.A("Will init module");
            ((DnnModule)context.Module).Init(dnnModule);
            return InitPageOnly(context, pageId);
        });

        private IContextOfBlock InitPageOnly(IContextOfBlock context, int? pageId) => Log.Func(() =>
        {
            // Collect / assemble page information
            var activeTab = (context.Site as Site<PortalSettings>)?.GetContents()?.ActiveTab;
            var page = (DnnPage)context.Page;
            var url = page.InitPageIdAndUrl(activeTab, pageId);
            return (context, url);
        });

    }
}
