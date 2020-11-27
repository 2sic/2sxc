using ToSic.Eav.Documentation;


// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
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
