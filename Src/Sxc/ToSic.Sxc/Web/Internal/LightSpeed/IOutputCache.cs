using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IOutputCache: IHasLog
{
    bool IsEnabled { get; }

    IOutputCache Init(int moduleId, int pageId, IBlock block);

    OutputCacheItem Existing { get; }

    bool Save(IRenderResult data);

#if NETFRAMEWORK
    bool Save(IRenderResult data, bool enforcePre1025);
#endif
}