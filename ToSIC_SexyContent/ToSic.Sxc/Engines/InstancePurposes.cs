namespace ToSic.Sxc.Engines
{
    public enum InstancePurposes
    {
        /// <summary>
        /// This is a normal use case, web-view.
        /// </summary>
        /// <remarks>The ID 0 on Webview must stay, because of compatibility with old code. </remarks>
        WebView = 0,

        /// <summary>
        /// This means the instance was created for the search-indexer to build the index.
        /// </summary>
        /// <remarks>The ID 1 on IndexingForSearch must stay, because of compatibility with old code. </remarks>
        IndexingForSearch = 1,

        /// <summary>
        /// This means the instance was only created to publish data as a JSON stream.
        /// This is a special use case and not documented well ATM.
        /// </summary>
        /// <remarks>The ID 2 on PublishData must stay, because of compatibility with old code. </remarks>
        PublishData = 2
    }
}