﻿namespace ToSic.Sxc.Apps.Sys.Work;

// Note: Unclear if this worker makes any sense, or should just be dropped.
// ATM it only has delete-entity without real view-specific logic

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkViewsMod(GenWorkPlus<WorkViews> appViews, GenWorkDb<WorkEntityDelete> entityDelete)
    : WorkUnitBase<IAppWorkCtx>("AWk.EntCre", connect: [appViews, entityDelete])
{

    public bool DeleteView(int viewId)
    {
        // really get template first, to be sure it is a template
        var template = appViews.New(AppWorkCtx).Get(viewId);
        return entityDelete.New(AppWorkCtx).Delete(template.Id);
    }
}