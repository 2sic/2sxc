using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockPublishAction(AppWorkContextService appWorkCtxSvc, WorkEntityPublish publisher) : ServiceBase("Sxc.BlPub", connect: [appWorkCtxSvc, publisher])
{
    public bool Publish(IBlock block, string part, int index)
    {
        var l = Log.Fn<bool>($"publish part{part}, order:{index}");
        var contEntity = block.Configuration[part][index];
        var presKey = part.ToLowerInvariant() == ViewParts.ContentLower
            ? ViewParts.PresentationLower
            : ViewParts.ListPresentationLower;
        var presEntity = block.Configuration[presKey][index];

        var appReader = block.Context.AppReaderRequired;
        var publishIds = new[] { contEntity, presEntity }
            // make sure we really have the draft item and not the live one
            .Select(e => e?.IsPublished == true ? appReader.GetDraft(e) : e)
            .OfType<IEntity>()
            .Select(e => e.RepositoryId)
            .ToArray();

        // This must happen within the using context, otherwise the appReader will not be able to find the draft entity
        var ctx = appWorkCtxSvc.ContextNew(block.Context.AppReaderRequired);
        publisher.Publish(ctx, publishIds);

        return l.ReturnTrue();
    }

}