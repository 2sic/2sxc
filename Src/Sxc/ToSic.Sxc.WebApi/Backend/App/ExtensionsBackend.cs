using System.Text.Json;
using ToSic.Sxc.Backend.Admin;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsBackend(
    LazySvc<ExtensionsReaderBackend> readerLazy,
    LazySvc<ExtensionsWriterBackend> writerLazy,
    LazySvc<ExtensionsZipInstallerBackend> zipLazy)
    : ServiceBase("Bck.Exts", connect: [readerLazy, writerLazy, zipLazy])
{
    public ExtensionsResultDto GetExtensions(int appId)
        => readerLazy.Value.GetExtensions(appId);

    public bool SaveExtension(int zoneId, int appId, string name, JsonElement configuration)
        => writerLazy.Value.SaveExtension(appId, name, configuration);

    public bool InstallExtensionZip(int zoneId, int appId, Stream zipStream, bool overwrite = false, string? originalZipFileName = null)
        => zipLazy.Value.InstallExtensionZip(appId, zipStream, overwrite, originalZipFileName);
}
