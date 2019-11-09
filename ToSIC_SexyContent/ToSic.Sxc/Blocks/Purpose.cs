namespace ToSic.Sxc.Blocks
{
    public enum Purpose
    {
        /// <summary>
        /// This is a normal use case, web-view.
        /// </summary>
        WebView,

        /// <summary>
        /// This means the instance was created for the search-indexer to build the index.
        /// </summary>
        IndexingForSearch,

        /// <summary>
        /// This means the instance was only created to publish data as a JSON stream.
        /// This is a special use case and not documented well ATM.
        /// </summary>
        PublishData
    }
}