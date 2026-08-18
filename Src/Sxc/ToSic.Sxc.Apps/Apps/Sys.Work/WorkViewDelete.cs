namespace ToSic.Sxc.Apps.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkViewDelete(AppWorkChain<WorkViews> appViews, AppWorkChain<WorkEntityDelete> entityDelete)
    : ServiceBase("AWk.EntCre", connect: [appViews, entityDelete])
{

    public bool DeleteView(IAppWorkContext appWorkCtx, int viewId)
    {
        // really get template first, to be sure it is a template
        var template = appViews.New(appWorkCtx).Get(viewId);
        return entityDelete.New(appWorkCtx).Delete(template.Id);
    }
}