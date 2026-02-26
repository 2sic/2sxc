namespace ToSic.Sxc.Backend.Cms.Load.Activities;
public record EditLoadActContext(int AppId, IAppReader AppReader, IContextOfApp AppContext);

public record EditLoadActContextWithWork(int AppId, IAppReader AppReader, IAppWorkCtxPlus AppWorkCtx, IContextOfApp AppContext)
    : EditLoadActContext(AppId, AppReader, AppContext);

public record EditLoadActContextWithUsedTypes(
    int AppId,
    IAppReader AppReader,
    IAppWorkCtxPlus AppWorkCtx,
    IContextOfApp AppContext,
    List<IContentType> UsedTypes) : EditLoadActContextWithWork(AppId, AppReader, AppWorkCtx, AppContext)
{

    public static EditLoadActContextWithUsedTypes Map(EditLoadActContextWithWork original, List<IContentType> usedTypes)
        => new(
            original.AppId,
            original.AppReader,
            original.AppWorkCtx,
            original.AppContext,
            usedTypes);
};