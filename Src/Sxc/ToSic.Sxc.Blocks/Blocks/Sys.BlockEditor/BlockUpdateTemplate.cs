using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockUpdateTemplate(LazySvc<AppWorkContextService> appWorkCtxSvc, AppWorkChain<WorkBlocksMod> workBlocksMod, BlockSavePicker savePicker)
    : ServiceBase("Sxc.BlkEdt", connect: [appWorkCtxSvc, workBlocksMod, savePicker]), IWork<(IBlock block, int templateId, bool forceCreateContentGroup), Guid?>
{
    public Task<Package<Guid?>> Handle(WorkContext mainCtx, Package<(IBlock block, int templateId, bool forceCreateContentGroup)> package)
    {
        var (block, templateId, forceCreateContentGroup) = package.Data;
        var result = SaveTemplateId(block, templateId, forceCreateContentGroup);
        return Task.FromResult(new Package<Guid?>(result));
    }

    /// <summary>
    /// Lookup the template and update the stored data; if necessary, create the content-group.
    /// </summary>
    /// <param name="templateId"></param>
    /// <param name="forceCreateContentGroup"></param>
    /// <returns></returns>
    private Guid? SaveTemplateId(IBlock block, int templateId, bool forceCreateContentGroup)
    {
        var blockConfig = block.Configuration;
        var updater = savePicker.PickUpdater(block);
        var l = Log.Fn<Guid?>($"save template#{templateId}, CG-exists:{blockConfig.Exists} forceCreateCG:{forceCreateContentGroup}");

        // if it exists or has a force-create, then write to the Content-Group, otherwise it's just a preview
        if (blockConfig.Exists || forceCreateContentGroup)
        {
            var existedBeforeSettingTemplate = blockConfig.Exists;
            var contentGroupGuid = workBlocksMod
                .New(appWorkCtxSvc.Value.ContextNew(block.Context.AppReaderRequired))
                .UpdateOrCreateContentGroup(blockConfig, templateId);

            if (!existedBeforeSettingTemplate)
                updater.SetContentGroup(block.Context.Module.Id, true, contentGroupGuid);

            return l.ReturnAndLog(contentGroupGuid);
        }

        // only set preview / content-group-reference - but must use the guid
        var templateGuid = block.App.AppReader.List.GetOne(templateId)!.EntityGuid;
        updater.SetPreview(block.Context.Module.Id, templateGuid);
        return l.Return(null, "only set preview, return null");
    }

}