using ToSic.Eav.Context;
using ToSic.Eav.Context.Internal;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// This provides other systems with a context
/// Note that it's important to always make this **Scoped**, not transient, as there is some re-use after initialization.
///
/// This is the reduced version, which can only provide context for the App!
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ISxcAppCurrentContextService: ICurrentContextService
{
    /// <summary>
    /// Return the block if known, or an app context if not
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    IContextOfApp GetExistingAppOrSet(int appId);

    IContextOfApp SetAppOrGetBlock(string nameOrPath);

    IContextOfApp? SetAppOrNull(string? nameOrPath);

    IContextOfApp AppNameRouteBlock(string? nameOrPath);
}