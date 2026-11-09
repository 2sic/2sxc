using ToSic.Sxc.Blocks.Sys.BlockEditor;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Do various publish operations, with additional security checks.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class PublishWebApi(
    LazySvc<IContextCompleteService> ctxService,
    PermissionCheckAction permCheckAction,
    LazySvc<WorkEntityPublish> workPublish,
    AppWorkContextService appWorkCtxService,
    LazySvc<BlockPublishAction> blockPublishAction)
    : ServiceBase("Api.EditRl", connect: [ctxService, permCheckAction, workPublish, appWorkCtxService])
{
    public const string LogSuffix = "Edit";

    public async Task<bool> Publish(int id)
    {
        var l = Log.Fn<bool>($"{id}");
        var block = ctxService.Value.BlockRequired();
        await permCheckAction.Handle(new(), new((block.Context, GrantSets.WritePublished)));
        var appCtx = appWorkCtxService.ContextNew(block.Context.AppReaderRequired);
        workPublish.Value.Publish(appCtx, [id]);
        return l.ReturnTrue("ok");
    }

    public async Task<bool> PublishPart(string part, int index)
    {
        var l = Log.Fn<bool>($"try to publish #{index} on '{part}'");
        var block = ctxService.Value.BlockRequired();
        await permCheckAction.Handle(new(), new((block.Context, GrantSets.WritePublished)));
        var ok = blockPublishAction.Value
            .Publish(block, part, index);
        return l.Return(ok, ok.ToString());
    }

}