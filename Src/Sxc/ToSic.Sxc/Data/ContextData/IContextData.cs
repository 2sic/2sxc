using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// The main data source for Blocks (Razor, WebApi).
    /// Internally often uses <see cref="CmsBlock"/> to find what it should provide.
    /// It's based on the <see cref="PassThrough"/> data source, because it's just a wrapper.
    /// </summary>
    /// <remarks>
    /// Introduced in v16.01 to simplify the API when using <see cref="ITypedItem"/>s.
    /// </remarks>
    [PublicApi]
    public interface IContextData: IBlockDataSource
    {
        /// <summary>
        /// The default/first item which _belongs_ to this block,
        /// aka `Content` which was edited by the user on this block.
        ///
        /// In most scenarios it's the same as the first Entity in this DataSource.
        /// But not when this block uses a Query, in which case the Entities of the DataSource return what's in the Query,
        /// and this returns the first/main item belonging to the block (eg. for configurations, titles, etc.)
        ///
        /// * it may also contain a demo item if the view is configured to use demo-data and no data was added by the user
        /// * if you need all the items in the `Default` stream, use the `List` property.
        /// </summary>
        /// <remarks>
        /// * New in v16.01
        /// * This `Data.Content` is an IEntity, which is different from the root `Content` object
        /// * This `Data.Content` is always the module/block content, never query data
        /// </remarks>
        IEntity Content { get; }

        /// <summary>
        /// The header item which _belongs_ to this block.
        /// This is usually something to configure what this block does, in addition to normal content items.
        ///
        /// * this only returns data if the **View** was configured to have `Header` data
        /// * it may also contain a demo item if no data was added by the user
        /// </summary>
        /// <returns>The header entity or null if the view doesn't have header entities.</returns>
        /// <remarks>
        /// * New in v16.01
        /// * This `Data.Header` is an IEntity, which is different from the root `Header` object
        /// </remarks>
        IEntity Header { get; }

        [PrivateApi("maybe just add for docs")]
        IReadOnlyDictionary<string, IDataStream> Out { get; }
    }
}
