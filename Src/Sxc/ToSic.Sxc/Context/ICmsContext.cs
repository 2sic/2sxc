using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
{
    /// <summary>
    /// This is the runtime context of your code in the CMS. It can tell you about the site, page, module etc. that you're on.
    /// Note that it it _Platform Agnostic_ so it's the same on Dnn, Oqtane etc.
    /// </summary>
    [PublicApi]
    public interface ICmsContext
    {
        /// <summary>
        /// Information about the Site (called Portal in DNN)
        /// </summary>
        ISiteLight Site { get; }

        /// <summary>
        /// Information about the Page (called Tab in DNN)
        /// </summary>
        IPageLight Page { get; }

        /// <summary>
        /// Information about the Module / Container which holds an 2sxc content block in the CMS
        /// </summary>
        IModuleLight Module { get; }

        /// <summary>
        /// Information about the current user
        /// </summary>
        IUserLight User { get; }
    }
}
