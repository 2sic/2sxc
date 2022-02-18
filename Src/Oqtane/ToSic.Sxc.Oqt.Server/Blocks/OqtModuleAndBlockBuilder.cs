using Oqtane.Models;
using Oqtane.Repository;
using System;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Oqt.Server.Blocks
{
    public class OqtModuleAndBlockBuilder : ModuleAndBlockBuilder
    {
        public OqtModuleAndBlockBuilder(
            Generator<IModule> moduleGenerator,
            Generator<IContextOfBlock> contextGenerator,
            Generator<BlockFromModule> blockGenerator,
            Generator<IModuleRepository> moduleRepositoryGenerator,
            RequestHelper requestHelper,
            Generator<ILinkPaths> linkPathsGenerator
        ) : base(OqtConstants.OqtLogPrefix)
        {
            _moduleGenerator = moduleGenerator;
            _contextGenerator = contextGenerator;
            _blockGenerator = blockGenerator;
            _moduleRepositoryGenerator = moduleRepositoryGenerator;
            _requestHelper = requestHelper;
            _linkPathsGenerator = linkPathsGenerator;
        }

        private readonly Generator<IModule> _moduleGenerator;
        private readonly Generator<IContextOfBlock> _contextGenerator;
        private readonly Generator<BlockFromModule> _blockGenerator;
        private readonly Generator<IModuleRepository> _moduleRepositoryGenerator;
        private readonly RequestHelper _requestHelper;
        private readonly Generator<ILinkPaths> _linkPathsGenerator;
        private ILog ParentLog => Log.Parent ?? Log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId">not required in Oqtane</param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        protected override IModule GetModuleImplementation(int pageId, int moduleId)
        {
            var oqtModule = (_moduleRepositoryGenerator.New).GetModule(moduleId);
            ThrowIfModuleIsNull(pageId, moduleId, oqtModule);
            var module = ((OqtModule) _moduleGenerator.New).Init(oqtModule, Log);
            return module;
        }

        protected override IBlock GetBlock(IModule module) => GetBlock((module as OqtModule)?.GetContents());

        public override IBlock GetBlock<TPlatformModule>(TPlatformModule module)
        {
            var wrapLog = Log.Call<BlockFromModule>();
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (!(module is Module oqtModule)) throw new ArgumentException("Given data is not a module");
            Log.Add($"Module: {oqtModule.ModuleId}");

            var initializedCtx = InitOqtSiteModuleAndBlockContext(oqtModule);
            var result = _blockGenerator.New.Init(initializedCtx, ParentLog);
            return wrapLog("ok", result);
        }

        private IContextOfBlock InitOqtSiteModuleAndBlockContext(Module oqtModule)
        {
            var wrapLog = Log.Call<IContextOfBlock>();
            var context = _contextGenerator.New;
            context.Init(ParentLog);
            //Log.Add($"Will try-swap module info of {oqtModule.ModuleId} into site");
            //((OqtSite)context.Site).TrySwap(oqtModule, ParentLog);
            Log.Add("Will init module");
            ((OqtModule) context.Module).Init(oqtModule, ParentLog);
            return wrapLog(null, InitPageOnly(context));
        }

        private IContextOfBlock InitPageOnly(IContextOfBlock context)
        {
            var wrapLog = Log.Call<IContextOfBlock>();
            // Collect / assemble page information
            context.Page.Init(_requestHelper.TryGetPageId());
            var url = (_linkPathsGenerator.New).GetCurrentRequestUrl();
            return wrapLog(url, context);
        }
    }
}
