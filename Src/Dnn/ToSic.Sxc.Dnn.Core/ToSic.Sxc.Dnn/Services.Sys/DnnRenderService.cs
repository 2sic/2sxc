using System.Web;
using ToSic.Eav.Data;
using ToSic.Razor.Markup;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Services;
using Page = System.Web.UI.Page;

namespace ToSic.Sxc.Dnn.Services.Sys;

internal class DnnRenderService(
    RenderService originalRenderService,
    LazySvc<DnnPageChanges> dnnPageChanges,
    LazySvc<DnnClientResources> dnnClientResources,
    Generator<IModuleFactory> context)
    : ServiceBase("Dnn.RndSvc", connect: [originalRenderService, dnnPageChanges, dnnClientResources, context]),
        IRenderService
{
    public IRawHtmlString One(ICanBeItem parent, NoParamOrder npo = default, ICanBeEntity item = null, object data = null,
        string field = null, Guid? newGuid = null)
        => originalRenderService.One(parent, npo, item, data, field, newGuid);

    public IRawHtmlString All(ICanBeItem parent, NoParamOrder npo = default, string field = null, string apps = null,
        int max = 100, string merge = null)
        => originalRenderService.All(parent, npo, field, apps, max, merge);

    public IRenderResult Module(int pageId, int moduleId, NoParamOrder npo = default, object data = null)
    {
        var l = Log.Fn<IRenderResult>($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}");
        var result = originalRenderService.Module(pageId, moduleId, npo, data);

        // this code should be executed in PreRender of page (ensure when calling) or it is too late
        if (HttpContext.Current?.Handler is Page dnnHandler) // detect if we are on the page
            if (context.New().Get(moduleId).BlockIdentifier == null) // find if is in module (because in module it's already handled)
                DnnPageProcess(dnnHandler, result);

        return l.ReturnAsOk(result);
    }

    private void DnnPageProcess(Page dnnPage, IRenderResult result)
    {
        dnnPageChanges.Value.Apply(dnnPage, result);
        dnnClientResources.Value.Init(dnnPage, null).AddEverything(result.Features);
    }
}