using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// This is kind of the master-container for a content-management block. It's the wrapper which is in the CMS (DNN), and the module will talk with this
    /// Sxc Block to get everything rendered. 
    /// </summary>
    [PrivateApi("not sure yet what to call this, CmsBlock isn't right, because it's more of a BlockHost or something")]
    public interface IBlockBuilder: IHasLog
    {
        /// <summary>
        /// Render this block. Internally will use the engine. 
        /// </summary>
        /// <returns></returns>
        string Render();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topLevel">
        /// This means it's the outer-most render which is happening.
        /// This changes if things like header changes, features etc. are picked up - which should only happen at top level
        /// </param>
        /// <returns></returns>
        [PrivateApi]
        RenderResult Run(bool topLevel);

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

        List<ClientAssetInfo> Assets { get; }
    }
}
