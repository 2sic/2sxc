using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Sxc.Backend.Admin;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsBackend(LazySvc<ExtensionsReaderBackend> readerLazy,
    LazySvc<ExtensionsWriterBackend> writerLazy,
    LazySvc<ExtensionsZipInstallerBackend> zipLazy,
    LazySvc<ExtensionsInspectorBackend> inspectorLazy)
    : ServiceBase("Bck.Exts", connect: [readerLazy, writerLazy, zipLazy, inspectorLazy])
{
    public ExtensionsResultDto GetExtensions(int appId)
        => readerLazy.Value.GetExtensions(appId);

    public bool SaveExtension(int zoneId, int appId, string name, ExtensionManifest manifest)
        => writerLazy.Value.SaveExtension(appId, name, manifest);

    public bool InstallExtensionZip(int zoneId, int appId, Stream zipStream, bool overwrite = false, string? originalZipFileName = null)
        => zipLazy.Value.InstallExtensionZip(appId, zipStream, overwrite, originalZipFileName);

    public ExtensionInspectResultDto InspectExtension(int appId, string name, string? edition)
        => inspectorLazy.Value.Inspect(appId, name, edition);
}
