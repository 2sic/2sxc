using ToSic.Eav.DataSource;

namespace ToSic.Sxc.DataSources;

// Internal notes
// * Introduced in v16.01 to simplify the API when using <see cref="ITypedItem"/>s.
// * Internally often uses <see cref="CmsBlock"/> to find what it should provide.
// * It's based on the <see cref="PassThrough"/> data source, because it's just a wrapper.
// * Was hidden till v19.00 and previously called `IBlockInstance`
// * published again iv 19.00 and renamed to `IBlockData` to better reflect what it does.

/// <summary>
/// The main data source for a View (Razor, Tokens).
/// Depending on the view configuration, it will either provide the data from a Query or from items added/edited on the Block itself.
/// </summary>
/// <remarks>
/// * Was hidden till v19.00 and previously called `IBlockInstance`, then renamed to `IBlockData` to better reflect what it does.
/// </remarks>
[PrivateApi("Was public from 16.01 till 17.00, but no real reason for it, so not any more")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IBlockInstance: IDataSource
{
}