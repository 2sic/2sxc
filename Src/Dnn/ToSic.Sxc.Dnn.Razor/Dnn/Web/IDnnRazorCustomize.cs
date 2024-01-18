using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Code;

using ToSic.Sxc.Search;

namespace ToSic.Sxc.Dnn.Web;

/// <summary>
/// All DNN Razor Pages inherit from this class
/// </summary>
[PrivateApi("used to be public till 16.09, but all methods were marked as obsolete a long time ago")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
public interface IDnnRazorCustomize: IDnnDynamicCode
{
    /// <summary>
    /// Override this to have your code change the (already initialized) Data object. 
    /// If you don't override this, nothing will be changed/customized. 
    /// </summary>
    [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
    void CustomizeData();

    /// <summary>
    /// Customize how the search will process data on this page. 
    /// </summary>
    /// <param name="searchInfos"></param>
    /// <param name="moduleInfo"></param>
    /// <param name="beginDate"></param>
    [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
    void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate);

    /// <summary>
    /// The purpose of the current execution. The code might be called for showing to a user, or search-indexing.
    /// </summary>
    /// <returns>The value of the current purpose.</returns>
    [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
    Purpose Purpose { get; }

}