
using static ToSic.Eav.Apps.Sys.Work.WorkEntityRecycleBin;

namespace ToSic.Sxc.Backend.Admin;

public interface IAdminDataController
{
    /// <summary>
    /// Bundle Export
    /// </summary>
    THttpResponseType BundleExport(int appId, Guid exportConfiguration, int indentation = 0);

    /// <summary>
    /// Bundle Import
    /// </summary>
    ImportResultDto BundleImport(int zoneId, int appId);

    /// <summary>
    /// Bundle Save
    /// </summary>
    bool BundleSave(int appId, Guid exportConfiguration, int indentation = 0);

    /// <summary>
    /// Bundle Restore
    /// </summary>
    bool BundleRestore(string fileName, int zoneId, int appId);

    /// <summary>
    /// Return all history entries, which can be recycled
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IReadOnlyList<RecycleBinItem> GetRecycleBin(int appId);

    /// <summary>
    /// Recycle a specific item
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="transactionId"></param>
    /// <exception cref="NotImplementedException"></exception>
    void Recycle(int appId, int transactionId);
}