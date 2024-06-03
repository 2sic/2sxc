using ToSic.Eav.Context;
using ToSic.Eav.Context.Internal;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// This provides other systems with a context
/// Note that it's important to always make this **Scoped**, not transient, as there is some re-use after initialization
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ISxcContextResolver: IContextResolver
{
    /// <summary>
    /// Return the block or throw an error
    /// </summary>
    IContextOfBlock BlockContextRequired();

    /// <summary>
    /// Return the block if known, or null if not
    /// </summary>
    /// <returns>The current block or null</returns>
    IContextOfBlock BlockContextOrNull();

    /// <summary>
    /// Return the block if known, or an app context if not
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    IContextOfApp GetBlockOrSetApp(int appId);

    IContextOfApp SetAppOrGetBlock(string nameOrPath);

    IContextOfApp SetAppOrNull(string nameOrPath);

    IContextOfApp AppNameRouteBlock(string nameOrPath);

    void AttachBlock(IBlock block);

    IBlock BlockOrNull();

    IBlock BlockRequired();

}