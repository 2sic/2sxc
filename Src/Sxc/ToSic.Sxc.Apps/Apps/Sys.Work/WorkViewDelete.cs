namespace ToSic.Sxc.Apps.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkViewDelete(GenWorkPlus<WorkViews> appViews, GenWorkDb<WorkEntityDelete> entityDelete, IAppWorkCtxForDiWip appWorkCtx)
    : ServiceBase("AWk.EntCre", connect: [appViews, entityDelete, appWorkCtx])
{

    public bool DeleteView(int viewId)
    {
        // really get template first, to be sure it is a template
        var template = appViews.New(appWorkCtx).Get(viewId);
        return entityDelete.New(appWorkCtx).Delete(template.Id);
    }
}