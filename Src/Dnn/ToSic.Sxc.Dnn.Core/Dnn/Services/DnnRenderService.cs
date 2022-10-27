using System;
using System.Web.UI;
using ToSic.Eav;
using ToSic.Eav.DI;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Services
{
    public class DnnRenderService : RenderService
    {
        private readonly Lazy<DnnPageChanges> _dnnPageChanges;
        private readonly Lazy<DnnClientResources> _dnnClientResources;

        public DnnRenderService(
            GeneratorLog<IEditService> editGenerator,
            LazyInitLog<IModuleAndBlockBuilder> builder,
            GeneratorLog<BlockFromEntity> blkFrmEntGen,
            Lazy<LogHistory> historyLazy,
            Lazy<DnnPageChanges> dnnPageChanges,
            Lazy<DnnClientResources> dnnClientResources

        ) : base(editGenerator, builder, blkFrmEntGen, historyLazy)
        {
            _dnnPageChanges = dnnPageChanges;
            _dnnClientResources = dnnClientResources;
        }

        public override IRenderResult Module(int pageId, int moduleId, string noParamOrder = Parameters.Protector, object page = default)
        {
            Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(All), $"{nameof(page)}");
            var wrapLog = Log.Fn<IRenderResult>($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}");
            var result = base.Module(pageId, moduleId);

            // this code should be executed in PreRender of page (ensure when calling) or it is too late
            if (page is Page dnnPage)
            {
                _dnnPageChanges.Value.Apply(dnnPage, result);
                _dnnClientResources.Value.Init(dnnPage, null, null, Log).AddEverything(result.Features);
            }

            return wrapLog.ReturnAsOk(result);
        }
    }
}
