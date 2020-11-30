using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    [PublicApi]
    public interface ISiteLight
    {
        /// <summary>
        /// The Id of the site in systems like DNN and Oqtane.
        /// In DNN this is the same as the PortalId
        /// </summary>
        int Id { get; }

    }
}
