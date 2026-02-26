namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public record EditLoadActivityContext(int AppId, IAppReader AppReader, IAppWorkCtxPlus AppWorkCtx, IContextOfApp AppContext);