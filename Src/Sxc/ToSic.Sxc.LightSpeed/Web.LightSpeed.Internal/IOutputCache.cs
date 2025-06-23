using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IOutputCache: IHasLog
{
    bool IsEnabled { get; }

    IOutputCache Init(int moduleId, int pageId, IBlock block);

    OutputCacheItem? Existing { get; }

    bool Save(IRenderResult data);

    // #RemovedV20 #OldDnnAntiForgery
    //#if NETFRAMEWORK
    //    bool Save(IRenderResult data, bool enforcePre1025);
    //#endif
}