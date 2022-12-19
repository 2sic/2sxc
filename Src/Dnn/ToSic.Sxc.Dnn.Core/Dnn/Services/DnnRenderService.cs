using System;
using System.Web;
using ToSic.Eav.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Services;
using Page = System.Web.UI.Page;

namespace ToSic.Sxc.Dnn.Services
{
    public class DnnRenderService : RenderService
    {
        private readonly Lazy<DnnPageChanges> _dnnPageChanges;
        private readonly Lazy<DnnClientResources> _dnnClientResources;
        private readonly Generator<IContextOfBlock> _context;

        public DnnRenderService(
            GeneratorLog<IEditService> editGenerator,
            LazyInitLog<IModuleAndBlockBuilder> builder,
            GeneratorLog<BlockFromEntity> blkFrmEntGen,
            Lazy<History> historyLazy,
            Lazy<DnnPageChanges> dnnPageChanges,
            Lazy<DnnClientResources> dnnClientResources,
            Generator<IContextOfBlock> context
        ) : base(editGenerator, builder, blkFrmEntGen, historyLazy)
        {
            _dnnPageChanges = dnnPageChanges;
            _dnnClientResources = dnnClientResources;
            _context = context;
        }

        public override IRenderResult Module(int pageId, int moduleId)
        {
            var wrapLog = Log.Fn<IRenderResult>($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}");
            var result = base.Module(pageId, moduleId);

            // this code should be executed in PreRender of page (ensure when calling) or it is too late
            if (HttpContext.Current?.Handler is Page dnnHandler) // detect if we are on the page
                if (_context.New().Module.BlockIdentifier == null) // find if is in module (because in module it's already handled)
                    DnnPageProcess(dnnHandler, result);

            return wrapLog.ReturnAsOk(result);
        }

        private void DnnPageProcess(Page dnnPage, IRenderResult result)
        {
            _dnnPageChanges.Value.Apply(dnnPage, result);
            _dnnClientResources.Value.Init(dnnPage, null, null, Log).AddEverything(result.Features);
        }
    }
}
