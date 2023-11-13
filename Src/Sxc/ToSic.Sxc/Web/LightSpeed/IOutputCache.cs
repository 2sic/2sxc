using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Web.LightSpeed
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IOutputCache: IHasLog
    {
        bool IsEnabled { get; }

        IOutputCache Init(int moduleId, int pageId, IBlock block);

        OutputCacheItem Existing { get; }

        OutputCacheItem Fresh { get; }

        bool Save(IRenderResult data);
    }
}
