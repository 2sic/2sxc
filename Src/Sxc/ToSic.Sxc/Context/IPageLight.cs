using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
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
