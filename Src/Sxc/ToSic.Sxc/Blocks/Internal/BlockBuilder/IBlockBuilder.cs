using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// This is kind of the master-container for a content-management block. It's the wrapper which is in the CMS (DNN), and the module will talk with this
/// Sxc Block to get everything rendered. 
/// </summary>
[PrivateApi("not sure yet what to call this, or if it should be public")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IBlockBuilder: IHasLog
{
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
    /// The root block, which controls what assets / js etc. will be rendered
    /// </summary>
    IBlockBuilder RootBuilder { get; }

    /// <summary>
    /// Determines if the output should be wrapped in a div or not
    /// </summary>
    bool WrapInDiv { get; set; }

    /// <summary>
    /// Get the engine which will render a block
    /// </summary>
    /// <returns></returns>
    IEngine GetEngine();
}