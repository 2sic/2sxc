using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.ImportExport;

namespace ToSic.Sxc.Backend.ImportExport;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExportApp(ExportHelper exportHelper) : ServiceBase("Bck.Export", connect: [exportHelper])
{
    public FileToUploadToClient Export(AppExportSpecs specs)
    {
        var l = Log.Fn<FileToUploadToClient>(specs.Dump());

        var (appReader, zipExport) = exportHelper.GetZipExportAndCheckZoneSwitchPermissions(specs);

        var addOnWhenContainingContent = specs.IncludeContentGroups
            ? "_withPageContent"
            : "";

        var fileName =
            $"2sxcApp{appReader.Specs.ToFileNameWithVersion()}{addOnWhenContainingContent}_{DateTime.Now:yyyy-MM-ddTHHmm}.zip";
        l.A($"file name:{fileName}");

        using var fileStream = zipExport.ExportApp(specs);
        var fileBytes = fileStream.ToArray();

        return l.Return(new()
        {
            FileName = fileName,
            ContentType = MimeTypeConstants.FallbackType,
            FileBytes = fileBytes
        }, $"will stream so many bytes: {fileBytes.Length}");
    }
}