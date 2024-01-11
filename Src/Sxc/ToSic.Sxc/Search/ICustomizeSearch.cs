using ToSic.Sxc.Context;

namespace ToSic.Sxc.Search;

/// <summary>
/// This interface marks custom code which views use to customize how search treats data of that view.
/// It's meant for customizing the internal indexer of the platform, _not_ for Google Search.
///
/// To use it, create a custom code (.cs) file which implements this interface.
/// You can also inherit from a DynamicCode base class (like Code12) if you need more functionality. 
/// </summary>
/// <remarks>
/// History: Released v12.02
/// </remarks>
[PublicApi]
public interface ICustomizeSearch
{
    /// <summary>
    /// Will be called by the search indexer to pre-process the results. 
    /// </summary>
    /// <param name="searchInfos">Dictionary containing the streams and items in the stream for this search.</param>
    /// <param name="moduleInfo">Module information with which you can find out what page it's on etc.</param>
    /// <param name="beginDate">Last time the indexer ran - because the data you will get is only what was modified since.</param>
    void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate);

}