#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Admin;

public interface IAdminDataController
{
    THttpResponseType BundleExport(int appId, Guid exportConfiguration, int indentation = 0);

    ImportResultDto BundleImport(int zoneId, int appId);
    //ImportResultDto BundleImport(HttpUploadedFile uploadInfo, int zoneId, int appId);

    bool BundleSave(int appId, Guid exportConfiguration, int indentation = 0);

    bool BundleRestore(int zoneId, int appId);
}