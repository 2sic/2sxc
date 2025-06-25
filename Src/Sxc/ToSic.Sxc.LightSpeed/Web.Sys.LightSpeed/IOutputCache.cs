using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Render.Sys;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IOutputCache: IHasLog
{
    bool IsEnabled { get; }

    IOutputCache Init(int moduleId, int pageId, IBlock block);

    OutputCacheItem? Existing { get; }

    bool Save(IRenderResult data);

    // #RemovedV20 #OldDnnAutoJQuery
    //#if NETFRAMEWORK
    //    bool Save(IRenderResult data, bool enforcePre1025);
    //#endif
}