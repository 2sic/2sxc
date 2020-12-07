using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// The site context of the code - so basically which website / portal it's running on. 
    /// </summary>
    [PublicApi]
    public interface ICmsSite
    {
        /// <summary>
        /// The Id of the site in systems like DNN and Oqtane.
        /// In DNN this is the same as the PortalId
        /// </summary>
        int Id { get; }

    }
}
