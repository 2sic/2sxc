using System.Web.Http.Controllers;
// using ToSic.Eav.Apps.Inspect.Sys.UiDto; // Legacy block-option endpoints.
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Backend.InPage;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Cms.BlockControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ValidateAntiForgeryToken]
// cannot use this, as most requests now come from a lone page [SupportedModules(DnnSupportedModuleNames)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockController : DnnApiController, IBlockController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion


    private RealController Real => SysHlp.GetServiceNoExCtx<RealController>();

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public string Block(int parentId, string field, int index, string app = "", Guid? guid = null)
    {
        SysHlp.PublishContextBlockOnly(Request, Log); 
        return Real.Block(parentId, field, index, app, guid);
    }


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public void Item([FromUri] int? index = null)
    {
        SysHlp.PublishContextBlockOnly(Request, Log);
        Real.Item(index);
    }


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public Task App(int? appId)
    {
        SysHlp.PublishContextBlockOnly(Request, Log);
        return Real.App(appId);
    }


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public Task<Guid?> Template(int templateId, bool forceCreateContentGroup)
    {
        SysHlp.PublishContextBlockOnly(Request, Log);
        return Real.Template(templateId, forceCreateContentGroup);
    }


    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public AjaxRenderDto Render([FromUri] int templateId, [FromUri] string lang, [FromUri] string edition)
    {
        SysHlp.PublishContextBlockOnly(Request, Log);
        return Real.Set(SysFolderRootVirtual.Trim('~')).Render(templateId, lang, edition);
    }

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public async Task<bool> Publish(string part, int index)
    {
        SysHlp.PublishContextBlockOnly(Request, Log);
        return await Real.Publish(part, index);
    }
}
