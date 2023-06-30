using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// This is the runtime context of your code in the CMS. It can tell you about the site, page, module etc. that you're on.
    /// Note that it it _Platform Agnostic_ so it's the same on Dnn, Oqtane etc.
    /// </summary>
    [PublicApi]
    public interface ICmsContext
    {
        /// <summary>
        /// Information about languages / culture of the current request
        /// </summary>
        ICmsCulture Culture { get; }

        /// <summary>
        /// Information about the Module / Container which holds an 2sxc content block in the CMS
        /// </summary>
        ICmsModule Module { get; }

        /// <summary>
        /// Information about the current Page (called Tab in DNN).
        /// It's especially useful to get current URL Parameters.
        /// </summary>
        ICmsPage Page { get; }

        /// <summary>
        /// Information about the platform that's currently running.
        /// </summary>
        ICmsPlatform Platform { get; }

        /// <summary>
        /// Information about the Site (called Portal in DNN)
        /// </summary>
        ICmsSite Site { get; }

        /// <summary>
        /// Information about the current user.
        /// It's especially useful to see if the user has any kind of Admin privileges.
        /// </summary>
        ICmsUser User { get; }
        
        
        /// <summary>
        /// Experimental feature for 12.02 - not final. Provides View-information.
        /// </summary>
        /// <remarks>New in v12.02, WIP</remarks>
        ICmsView View { get; }


        [PrivateApi("WIP v13")]
        ICmsBlock Block { get; }
    }
}
