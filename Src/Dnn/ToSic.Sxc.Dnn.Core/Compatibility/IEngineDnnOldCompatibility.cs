using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Search;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn.Web;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal interface IEngineDnnOldCompatibility
{
    bool OldAutoLoadJQueryAndRvt { get; }

    /// <summary>
    /// Initialize the Engine (pass everything needed for Render to it).<br/>
    /// This is not in the constructor, because IEngines usually get constructed with DI,
    /// so the constructor is off-limits. 
    /// </summary>
    /// <param name="block">block within the cms</param>
    /// <param name="purpose">Purpose of the engine (show in web, search-index, etc.). The custom code may adapt its behavior depending on the purpose</param>
#pragma warning disable CS0618
    void Init(IBlock block, Purpose purpose);
#pragma warning restore CS0618

    /// <summary>
    /// Mechanism which allows the view to change data it will show in a stream-based way.
    /// This helps to ensure that other parts like JSON-Streams or Search have the same information
    /// as the view itself. 
    /// </summary>
    [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
    void CustomizeData();

    /// <summary>
    /// Mechanism which lets the search indexer ask the template how it should pre-process the content.
    /// </summary>
    /// <param name="searchInfos"></param>
    /// <param name="moduleInfo"></param>
    /// <param name="beginDate"></param>
    [PrivateApi]
    [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
    void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate);

}