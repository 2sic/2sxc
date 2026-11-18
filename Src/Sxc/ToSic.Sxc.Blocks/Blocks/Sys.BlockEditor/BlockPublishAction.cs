using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockPublishAction(AppWorkQuick<WorkEntityPublish> publisher)
    : ServiceWithSetup<IAppWorkContext>("Sxc.BlPub", connect: [publisher])
{
    public bool Publish(BlockConfiguration blockConfig, string part, int index)
    {
        var l = Log.Fn<bool>($"publish part{part}, order:{index}");
        var contEntity = blockConfig[part][index];
        var presKey = part.ToLowerInvariant() == ViewParts.ContentLower
            ? ViewParts.PresentationLower
            : ViewParts.ListPresentationLower;
        var presEntity = blockConfig[presKey][index];

        var appReader = MyOptions.AppReader;
        var publishIds = new[] { contEntity, presEntity }
            // make sure we really have the draft item and not the live one
            .Select(e => e?.IsPublished == true ? appReader.GetDraft(e) : e)
            .OfType<IEntity>()
            .Select(e => e.RepositoryId)
            .ToArray();

        // This must happen within the using context, otherwise the appReader will not be able to find the draft entity
        publisher.New(appReader).Publish(publishIds);

        return l.ReturnTrue();
    }

}