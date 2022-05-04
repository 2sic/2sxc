using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Web.LightSpeed
{
    public interface IOutputCache: IHasLog
    {
        bool IsEnabled { get; }

        IOutputCache Init(int moduleId, IBlock block);

        OutputCacheItem Existing { get; }

        OutputCacheItem Fresh { get; }

        //bool IsInCache { get; }

        bool Save(IRenderResult data);
    }
}
