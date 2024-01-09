using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal.Render;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IOutputCache: IHasLog
{
    bool IsEnabled { get; }

    IOutputCache Init(int moduleId, int pageId, IBlock block);

    OutputCacheItem Existing { get; }

    OutputCacheItem Fresh { get; }

    bool Save(IRenderResult data);
}