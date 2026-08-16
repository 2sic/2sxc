using ToSic.Eav.Apps.Sys;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityAddContext(IUiContextBuilder contextBuilder): ServiceBase("UoW.AddCtx", connect: [contextBuilder]),
    IWork<EditLoadDto, EditLoadDto>
{
    public async Task<Package<EditLoadDto>> Handle(WorkContext actionCtx, Package<EditLoadDto> package)
    {
        var l = Log.Fn<Package<EditLoadDto>>();
        var isSystemType = actionCtx.Get<List<IContentType>>(EditLoadContextConstants.UsedTypes).Any(t => t.AppId == KnownAppsConstants.PresetAppId);
        l.A($"isSystemType: {isSystemType}");

        // Attach context, but only the minimum needed for the UI
        package = package with
        {
            Data = package.Data with
            {
                Context = contextBuilder.InitApp(actionCtx.Get<IAppReader>(EditLoadContextConstants.AppReader))
                    .Get(Ctx.AppBasic | Ctx.AppEdit | Ctx.Language | Ctx.Site | Ctx.System | Ctx.User | Ctx.UserRoles |
                         Ctx.Features |
                         (isSystemType ? Ctx.FeaturesForSystemTypes : Ctx.Features), CtxEnable.EditUi),

            },
        };
        return l.Return(package);
    }
}
