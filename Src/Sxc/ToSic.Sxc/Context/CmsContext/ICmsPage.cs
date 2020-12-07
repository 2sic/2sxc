using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Information about the page which is the context for the currently running code.
    /// </summary>
    /// <remarks>
    /// Note that the module context is the module for which the code is currently running.
    /// In some scenarios (like Web-API scenarios) the code is running _for_ this page but _not on_ this page,
    /// as it would then be running on a WebApi.
    /// </remarks>
    [PublicApi]
    public interface ICmsPage
    {
        /// <summary>
        /// The Id of the page.
        /// Corresponds to the Dnn TabId
        /// </summary>
        int Id { get; }

    }
}
