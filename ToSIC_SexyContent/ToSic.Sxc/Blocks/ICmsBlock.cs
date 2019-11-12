using System.Collections.Generic;
using System.Web;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Environment;
using ToSic.Eav.Logging;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// This is kind of the master-container for a content-management block. It's the wrapper which is in the CMS (DNN), and the module will talk with this
    /// Sxc Block to get everything rendered. 
    /// </summary>
    [PublicApi]
    public interface ICmsBlock: IHasLog, IAppIdentity
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
        /// Determines if the current user may edit content here.
        /// </summary>
        bool UserMayEdit { get; }

        [PrivateApi]
        IAppEnvironment Environment { get; }

        // todo: better name, this is kind of the module we're in or something
        [PrivateApi]
        IContainer EnvInstance { get; }

        [PrivateApi]
        IEnumerable<KeyValuePair<string, string>> Parameters { get; }

    }
}
