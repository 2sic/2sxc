using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
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
    public interface IContextData: IBlockDataSource, INeedsDynamicCodeRoot
    {
        /// <summary>
        /// The items which _belongs_ to this block,
        /// aka `Content` which was edited by the user on this block.
        ///
        /// In most scenarios it's the same as the first/all Entities in this DataSource.
        /// But not when this block uses a Query, in which case the Entities of the DataSource return what's in the Query,
        /// and this returns the first/main item belonging to the block (eg. for configurations, titles, etc.)
        ///
        /// * it may also contain a demo item if the view is configured to use demo-data and no data was added by the user
        /// * to get all the items in the `Default` stream (eg. from a Query), see the <see cref="MyData"/>
        /// * to get all items from another stream, use `GetStream(streamName, optionalParameters...)`
        /// </summary>
        /// <remarks>
        /// * New in v16.02
        /// * This `Data.MyContent` is a list of IEntity objects; the root `Content` object is a dynamic object
        /// * This `Data.MyContent` is always the module/block instance content, never query data
        /// </remarks>
        /// <returns>
        /// The list of all of the instance content items, OR an empty list.
        /// Should _never_ return `null`.
        /// </returns>
        IEnumerable<IEntity> MyContent { get; }


        /// <summary>
        /// The data items which which are in the `Default` stream.
        ///
        /// In many scenarios it's the same as <see cref="MyContent"/>.
        /// But not when this block uses a Query, in which case this contains the `Default` stream from the query.
        ///
        /// * to get all the items belonging to the current instance, see the <see cref="MyContent"/>
        /// * to get all items from another stream, use `GetStream(streamName, optionalParameters...)`
        /// </summary>
        /// <remarks>
        /// * New in v16.02
        /// * It's basically identical to the `List` property, but for API consistency we created the `MyData`
        /// </remarks>
        /// <returns>
        /// The list of items in the `Default` stream.
        /// Should _never_ return `null`.
        /// </returns>
        IEnumerable<IEntity> MyData { get; }

        /// <summary>
        /// The header item which _belongs_ to this block.
        /// This is usually something to configure what this block does, in addition to normal content items.
        ///
        /// * this only returns data if the **View** was configured to have `Header` data
        /// * it may also contain a demo item if no data was added by the user
        /// * it will normally only have zero or one (0/1) items, more or not expected, but it's an IEnumerable for consistency with <see cref="MyContent"/> and <see cref="MyData"/>
        /// </summary>
        /// <returns>A list of Entities with one item; zero items if the view doesn't have header entities.</returns>
        /// <remarks>
        /// * New in v16.02
        /// * This `Data.MyHeader` is a List of IEntity objects; the root `Header` is a dynamic object
        /// </remarks>
        /// <returns>
        /// A list containing a header item or an empty list.
        /// Should _never_ return `null`.
        /// </returns>
        IEnumerable<IEntity> MyHeader { get; }

        [PrivateApi("maybe just add for docs")]
        new IReadOnlyDictionary<string, IDataStream> Out { get; }
    }
}
