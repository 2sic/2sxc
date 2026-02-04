using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Render.Sys.Specs;

namespace ToSic.Sxc.Render.Sys.RenderBlock;

/// <summary>
/// This is kind of the master-container for a content-management block. It's the wrapper which is in the CMS (DNN), and the module will talk with this
/// Sxc Block to get everything rendered. 
/// </summary>
[PrivateApi("not sure yet what to call this, or if it should be public")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IBlockBuilder: IHasLog
{
    public IBlockBuilder Setup(IBlock cb);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="topLevel">
    ///     This means it's the outer-most render which is happening.
    ///     This changes if things like header changes, features etc. are picked up - which should only happen at top level
    /// </param>
    /// <param name="specs">Data to be added to the model of the main template/razor</param>
    /// <returns></returns>
    [PrivateApi]
    IRenderResult Run(bool topLevel, RenderSpecs specs);

    [PrivateApi]
    IRenderingHelper RenderingHelper { get; }

    /// <summary>
    /// The real block / unit of content which will be rendered. 
    /// </summary>
    IBlock Block { get; }

    /// <summary>
    /// Determines if the output should be wrapped in a div or not
    /// </summary>
    bool WrapInDiv { get; set; }

    ///// <summary>
    ///// Get the engine which will render a block
    ///// </summary>
    ///// <returns></returns>
    //IEngine? GetEngine();
}