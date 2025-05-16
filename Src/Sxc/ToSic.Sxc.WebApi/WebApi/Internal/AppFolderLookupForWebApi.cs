using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Apps.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppFolderLookupForWebApi(ISxcContextResolver ctxResolver) : ServiceBase("AppFld")
{
    /// <summary>
    /// This is necessary for special calls where the _ctxResolve may not yet be complete...
    /// Important: not sure if this is actually needed, I believe the ctxResolver is always initialized on all web-api requests...?
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public AppFolderLookupForWebApi Init(IBlock block)
    {
        ctxResolver.AttachBlock(block);
        return this;
    }

    public string GetAppFolder()
    {
        var ctx = ctxResolver.AppNameRouteBlock("");
        return ctx.AppReader.Specs.Folder;
    }
}