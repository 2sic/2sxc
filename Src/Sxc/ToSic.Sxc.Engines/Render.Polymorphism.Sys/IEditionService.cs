using ToSic.Eav.Apps;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Render.Polymorphism.Sys;

/// <summary>
/// Mini service to read the polymorph config of the app
/// and then resolve the edition based on an <see cref="IPolymorphismResolver"/>
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IEditionService
{
    /// <summary>
    /// Try to use the edition set by the view (like in preview mode)
    /// or to use this same PolymorphConfigReader to figure out the edition.
    /// Since the reader should only be created if necessary, it's handed in as a function.
    /// </summary>
    public string? Edition(IView? view, IAppReader appReader);

    /// <summary>
    /// Try to get the edition based on the block information.
    /// If the view is not ready, it will return null.
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    string? Edition(IBlock block);
}