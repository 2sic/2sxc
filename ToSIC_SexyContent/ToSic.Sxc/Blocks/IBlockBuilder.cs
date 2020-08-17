using ToSic.Eav.Apps.Run;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using IApp = ToSic.Sxc.Apps.IApp;
#if NET451
using HtmlString = System.Web.HtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
#endif

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
        /// The app relevant to this instance - contains much more material like
        /// app-path or all the data existing in this app
        /// </summary>
        IApp App { get; }


        /// <summary>
        /// The view in the current block - necessary to pick up the right rendering engine etc.
        /// </summary>
        /// <remarks>It usually pre-defined by the inner content-block, but in rare cases it can be overriden, for example when previewing a template switch.</remarks>
        IView View { get; }


        /// <summary>
        /// Render this block. Internally will use the engine. 
        /// </summary>
        /// <returns></returns>
        HtmlString Render();

        /// <summary>
        /// The real block / unit of content which will be rendered. 
        /// </summary>
        IBlock Block { get; }

        /// <summary>
        /// The root block, which controls what assets / js etc. will be rendered
        /// </summary>
        IBlockBuilder RootBuilder { get; }

        /// <summary>
        /// Determines if the current user may edit content here.
        /// </summary>
        bool UserMayEdit { get; }

        [PrivateApi]
        IAppEnvironment Environment { get; }

        /// <summary>
        /// The context in which this is running
        /// </summary>
        IInstanceContext Context { get; }

    }
}
