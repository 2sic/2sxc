using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Code.Internal;

public interface ICodeApiServiceFactory
{
    /// <summary>
    /// Creates a CodeApiService - if possible based on the parent class requesting it.
    /// </summary>
    /// <param name="parentClassOrNull"></param>
    /// <param name="blockOrNull"></param>
    /// <param name="parentLog"></param>
    /// <param name="compatibilityFallback"></param>
    /// <returns></returns>
    ICodeApiService New(object parentClassOrNull, IBlock blockOrNull, ILog parentLog, int compatibilityFallback);
}