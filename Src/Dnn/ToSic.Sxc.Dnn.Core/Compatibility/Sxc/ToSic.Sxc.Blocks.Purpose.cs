#if NETFRAMEWORK
namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// These are the purposes of a block as it's being built. It can be built to generate a web-view, juts for indexing or possibly also for json publishing. 
    /// </summary>
    [PrivateApi("Deprecated - avoid using - still used in older Dnn APIs - was public till 16.07, then private")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("Deprecated - avoid using - still used in older Dnn APIs")]
    public enum Purpose
    {
        /// <summary>
        /// This is a normal use case, web-view.
        /// </summary>
        WebView,

        /// <summary>
        /// The purpose is for the search-indexer to build the index.
        /// </summary>
        IndexingForSearch,
    }
}
#endif