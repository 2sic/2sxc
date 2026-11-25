using ToSic.Eav.Apps.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Blocks.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkBlocksUpdate(AppWorkChain<WorkEntityCreate> workEntCreate, AppWorkChain<WorkEntityUpdate> workEntUpdate)
    : ServiceWithSetup<IAppWorkContext>("AWk.EntCre", connect: [workEntCreate, workEntUpdate])
{
    public Guid UpdateOrCreateContentGroup(BlockConfiguration blockConfiguration, int templateId)
    {
        var l = Log.Fn<Guid>();

        return !blockConfiguration.Exists
            ? l.ReturnAndLog(Create(templateId), "created") :
            l.ReturnAndLog(Update(blockConfiguration, templateId));
    }

    private Guid Create(int templateId)
    {
        var l = Log.Fn<Guid>($"doesn't exist, will create new CG with template#{templateId}");
        var guid = workEntCreate.New(MyOptions)
            .Create(AppConstants.ContentGroupTypeName, new()
            {
                { ViewParts.TemplateContentType, new List<int> { templateId } },
            })
            .EntityGuid; // new guid
        return l.ReturnAndLog(guid, "created");
    }

    private Guid Update(BlockConfiguration blockConfig, int templateId)
    {
        var l = Log.Fn<Guid>($"exists, update for group#{blockConfig.Guid} with template#{templateId}");

        workEntUpdate.New(MyOptions).UpdateParts(
            blockConfig.Id,
            new Dictionary<string, object> { { ViewParts.TemplateContentType, new List<int?> { templateId } } },
            new()
        );

        return l.ReturnAndLog(blockConfig.Guid); // guid didn't change
    }
}
