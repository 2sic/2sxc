using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.WebApi.Sys.Entities;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityAddContext(IUiContextBuilder contextBuilder): ServiceBase("UoW.AddCtx", connect: [contextBuilder])
{
    public async Task<ActionData<EditLoadDto>> Run(LowCodeActionContext actionCtx, ActionData<EditLoadDto> result)
    {
        var l = Log.Fn<ActionData<EditLoadDto>>();
        var isSystemType = actionCtx.Get<List<IContentType>>(EditLoadContextConstants.UsedTypes).Any(t => t.AppId == KnownAppsConstants.PresetAppId);
        l.A($"isSystemType: {isSystemType}");

        // Attach context, but only the minimum needed for the UI
        result = result with
        {
            Data = result.Data with
            {
                Context = contextBuilder.InitApp(actionCtx.Get<IAppReader>(EditLoadContextConstants.AppReader))
                    .Get(Ctx.AppBasic | Ctx.AppEdit | Ctx.Language | Ctx.Site | Ctx.System | Ctx.User | Ctx.UserRoles |
                         Ctx.Features |
                         (isSystemType ? Ctx.FeaturesForSystemTypes : Ctx.Features), CtxEnable.EditUi),

            },
        };
        return l.Return(result);
    }
}
