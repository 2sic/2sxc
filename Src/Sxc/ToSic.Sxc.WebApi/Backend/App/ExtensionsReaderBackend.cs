using System.Text.Json.Nodes;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsReaderBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    LazySvc<IJsonService> jsonLazy)
    : ServiceBase("Bck.ExtRead", connect: [appReadersLazy, site, appPathSvc, jsonLazy])
{
    public ExtensionsResultDto GetExtensions(int appId)
    {
        var l = Log.Fn<ExtensionsResultDto>($"a#{appId}");
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var root = Path.Combine(appPaths.PhysicalPath, ToSic.Eav.Sys.FolderConstants.AppExtensionsFolder);

        var list = new List<ExtensionDto>();
        if (Directory.Exists(root))
        {
            foreach (var dir in Directory.GetDirectories(root))
            {
                var folderName = Path.GetFileName(dir);
                var appData = Path.Combine(dir, ToSic.Eav.Sys.FolderConstants.DataFolderProtected);
                var jsonPath = Path.Combine(appData, ToSic.Eav.Sys.FolderConstants.AppExtensionJsonFile);
                object? configuration = null;
                try
                {
                    if (File.Exists(jsonPath))
                    {
                        var json = File.ReadAllText(jsonPath);
                        configuration = jsonLazy.Value.ToObject(json);
                    }
                }
                catch (Exception ex)
                {
                    l.Ex(ex);
                }

                configuration ??= new JsonObject();
                list.Add(new ExtensionDto { Folder = folderName, Configuration = configuration });
            }
        }
        return l.ReturnAsOk(new ExtensionsResultDto { Extensions = list });
    }
}
