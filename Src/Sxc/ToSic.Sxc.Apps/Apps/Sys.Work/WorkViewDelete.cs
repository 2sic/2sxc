namespace ToSic.Sxc.Apps.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkViewDelete(Generator<WorkViews, IAppWorkCtxForDiWip> appViews, GenWorkDb<WorkEntityDelete> entityDelete)
    : ServiceBase("AWk.EntCre", connect: [appViews, entityDelete])
{

    public bool DeleteView(IAppWorkCtxForDiWip appWorkCtx, int viewId)
    {
        // really get template first, to be sure it is a template
        var template = appViews.New(appWorkCtx).Get(viewId);
        return entityDelete.New(appWorkCtx).Delete(template.Id);
    }
}