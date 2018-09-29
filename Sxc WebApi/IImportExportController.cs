using System.Net.Http;
using ToSic.Eav.WebApi.ImportExport;

namespace ToSic.SexyContent.WebApi
{
    public interface IImportExportController
    {
        HttpResponseMessage ExportApp(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid);
        HttpResponseMessage ExportContent(int appId, int zoneId, string contentTypeIdsString, string entityIdsString, string templateIdsString);
        bool ExportForVersionControl(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid);
        dynamic GetAppInfo(int appId, int zoneId);
        dynamic GetContentInfo(int appId, int zoneId, string scope);
        ImportResult ImportApp();
        ImportResult ImportContent();
    }
}