using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    [PublicApi]
    public interface IPageLight
    {
        /// <summary>
        /// The Id of the page.
        /// Corresponds to the Dnn TabId
        /// </summary>
        int Id { get; }

    }
}
