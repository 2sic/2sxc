using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockEditor;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Do various publish operations, with additional security checks.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class PublishWebApi(
    PermissionCheckAction permCheckAction,
    AppWorkQuick<WorkEntityPublish> workPublish,
    AppWorkQuick<BlockPublishAction> blockPublishAction)
    : ServiceBase("Api.EditRl", connect: [permCheckAction, workPublish, blockPublishAction])
{
    public const string LogSuffix = "Edit";

    public async Task<bool> Publish(IBlock block, int id)
    {
        var l = Log.Fn<bool>($"{id}");
        await permCheckAction.Handle(new(), new((block.Context, GrantSets.WritePublished)));
        workPublish.New(block.Context.AppReaderRequired).Publish([id]);
        return l.ReturnTrue("ok");
    }

    public async Task<bool> PublishPart(IBlock block, string part, int index)
    {
        var l = Log.Fn<bool>($"try to publish #{index} on '{part}'");
        await permCheckAction.Handle(new(), new((block.Context, GrantSets.WritePublished)));
        var ok = blockPublishAction.New(block)
            .Publish(block.Configuration, part, index);
        return l.Return(ok, ok.ToString());
    }

}