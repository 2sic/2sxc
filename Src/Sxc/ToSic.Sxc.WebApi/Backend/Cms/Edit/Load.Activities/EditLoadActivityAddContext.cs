using ToSic.Eav.Apps.Sys;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityAddContext(IUiContextBuilder contextBuilder): ServiceBase("UoW.AddCtx", connect: [contextBuilder])
{
    public EditLoadDto Run(EditLoadDto result, EditLoadActContextWithUsedTypes actionCtx)
    {
        var l = Log.Fn<EditLoadDto>();
        var isSystemType = actionCtx.UsedTypes.Any(t => t.AppId == KnownAppsConstants.PresetAppId);
        l.A($"isSystemType: {isSystemType}");

        // Attach context, but only the minimum needed for the UI
        result = result with
        {
            Context = contextBuilder.InitApp(actionCtx.AppReader)
                .Get(Ctx.AppBasic | Ctx.AppEdit | Ctx.Language | Ctx.Site | Ctx.System | Ctx.User | Ctx.UserRoles | Ctx.Features |
                     (isSystemType ? Ctx.FeaturesForSystemTypes : Ctx.Features), CtxEnable.EditUi),

            // Load settings for the front-end
            //Settings = loadSettings.GetSettings(context, usedTypes, result.ContentTypes, appWorkCtx),
        };
        return l.Return(result);
    }
}
