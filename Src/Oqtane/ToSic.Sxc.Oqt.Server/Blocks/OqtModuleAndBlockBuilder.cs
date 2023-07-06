using Oqtane.Models;
using Oqtane.Repository;
using System;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Infrastructure;
using Log = ToSic.Lib.Logging.Log;

namespace ToSic.Sxc.Oqt.Server.Blocks
{
    public class OqtModuleAndBlockBuilder : ModuleAndBlockBuilder
    {
        public OqtModuleAndBlockBuilder(
            Generator<IModule> moduleGenerator,
            Generator<IContextOfBlock> contextGenerator,
            Generator<BlockFromModule> blockGenerator,
            Generator<IModuleRepository> moduleRepositoryGenerator,
            RequestHelper requestHelper
        ) : base(blockGenerator, OqtConstants.OqtLogPrefix)
        {
            _moduleGenerator = moduleGenerator;
            _contextGenerator = contextGenerator;
            _moduleRepositoryGenerator = moduleRepositoryGenerator;
            _requestHelper = requestHelper;
        }

        private readonly Generator<IModule> _moduleGenerator;
        private readonly Generator<IContextOfBlock> _contextGenerator;
        private readonly Generator<IModuleRepository> _moduleRepositoryGenerator;
        private readonly RequestHelper _requestHelper;
        private ILog ParentLog => (Log as Log)?.Parent ?? Log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId">not required in Oqtane</param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        protected override IModule GetModuleImplementation(int pageId, int moduleId)
        {
            var oqtModule = (_moduleRepositoryGenerator.New()).GetModule(moduleId);
            ThrowIfModuleIsNull(pageId, moduleId, oqtModule);
            var module = ((OqtModule) _moduleGenerator.New()).Init(oqtModule);
            return module;
        }

        protected override IContextOfBlock GetContextOfBlock(IModule module, int? pageId) => GetContextOfBlock((module as OqtModule)?.GetContents(), pageId);

        protected override IContextOfBlock GetContextOfBlock<TPlatformModule>(TPlatformModule module, int? pageId)
        {
            var wrapLog = Log.Fn<IContextOfBlock>();
            if (module == null) throw new ArgumentNullException(nameof(module));

            var oqtModule = module switch
            {
                Module oModule => oModule,
                PageModule oPageModule => oPageModule.Module,
                _ => throw new ArgumentException("Given data is not a module")
            };

            Log.A($"Module: {oqtModule.ModuleId}");
            var initializedCtx = InitOqtSiteModuleAndBlockContext(oqtModule, pageId);
            return wrapLog.ReturnAsOk(initializedCtx);
        }


        private IContextOfBlock InitOqtSiteModuleAndBlockContext(Module oqtModule, int? pageId)
        {
            var wrapLog = Log.Fn<IContextOfBlock>();
            var context = _contextGenerator.New();
            Log.A("Will init module");
            ((OqtModule) context.Module).Init(oqtModule);
            return wrapLog.Return(InitPageOnly(context, pageId));
        }

        private IContextOfBlock InitPageOnly(IContextOfBlock context, int? pageId)
        {
            // TODO: try to use the pageId if given, would usually only be used in inner-content / IRenderService scenarios

            var wrapLog = Log.Fn<IContextOfBlock>();
            // Collect / assemble page information
            context.Page.Init(_requestHelper.TryGetPageId());
            var url = context.Page.Url;
            return wrapLog.Return(context, url);
        }
    }
}
