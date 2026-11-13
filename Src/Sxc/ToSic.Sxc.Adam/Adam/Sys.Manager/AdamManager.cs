using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Adam.Sys.FileSystem;
using ToSic.Sxc.Adam.Sys.Paths;
using ToSic.Sxc.Adam.Sys.Storage;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Adam.Sys.Manager;

/// <summary>
/// The Manager of ADAM
/// In charge of managing assets inside this app - finding them, creating them etc.
/// </summary>
/// <remarks>
/// It's abstract, because there will be a typed implementation inheriting this
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamManager(
    Generator<ICodeDataFactory, ICodeDataFactory.OptionalFallbacks> cdfIfNotProvided,
    AdamConfiguration adamConfiguration,
    Generator<IAdamFileSystem, AdamManager> adamFsLazy,
    Generator<AdamStorageOfField> fieldStorageGenerator,
    AdamGenericHelper adamGenericHelper)
    : ServiceWithSetup<AdamManager.Options>("Adm.Managr", connect: [cdfIfNotProvided, adamConfiguration, adamFsLazy, fieldStorageGenerator, adamGenericHelper])
{

    #region Init

    public record Options(IContextOfApp AppCtx, int Compatibility, ICodeDataFactory? Cdf = default);

    public bool UseTypedAssets => Cdf.CompatibilityLevel >= CompatibilityLevels.MinLevelForTyped;

    [field: AllowNull, MaybeNull] public IAdamFileSystem AdamFs => field ??= adamFsLazy.New(this);

    [field: AllowNull, MaybeNull]
    public IAppReader AppReader => field ??= AppContext.AppReaderRequired;

    private IContextOfApp AppContext => field ??= MyOptions.AppCtx;

    [field: AllowNull, MaybeNull]
    public ISite Site => field ??= AppContext.Site;

    internal ICodeDataFactory Cdf => field
        ??= MyOptions.Cdf
            ?? cdfIfNotProvided.New(new(MyOptions.AppCtx.Site, MyOptions.Compatibility, this));

    #endregion

    /// <summary>
    /// Path to the app assets
    /// </summary>
    [field: AllowNull, MaybeNull]
    public string Path => field ??= adamConfiguration.PathForApp(AppContext.AppReaderRequired.Specs);

    #region Folder Stuff

    /// <summary>
    /// Root folder object of the app assets
    /// </summary>
    public IFolder? RootFolder => Folder(Path, true);

    internal IFolder Folder(string path)
        => AdamFs.Get(path);

    internal IFolder? Folder(string path, bool autoCreate)
    {
        var l = Log.Fn<IFolder>($"{path}, {autoCreate}");

        // create all folders to ensure they exist. Must do one-by-one because the environment must have it in the catalog
        var pathParts = path.Split('/');
        var pathToCheck = "";
        foreach (var part in pathParts.Where(p => !string.IsNullOrEmpty(p)))
        {
            pathToCheck += part + "/";
            if (AdamFs.FolderExists(pathToCheck))
                continue;
            if (autoCreate)
                AdamFs.AddFolder(pathToCheck);
            else
            {
                Log.A($"subfolder {pathToCheck} not found");
                return l.ReturnNull("not found");
            }
        }

        return l.ReturnAsOk(Folder(path));
    }

    public IFolder FolderOfField(Guid entityGuid, string fieldName, IField? field = default)
    {
        var folderStorage = fieldStorageGenerator.New().InitItemAndField(entityGuid, fieldName);
        folderStorage.Init(this);
        var folder = adamGenericHelper.FolderOfField(this, folderStorage, field);
        return folder;
    }
    #endregion

    #region Metadata Maker

    /// <summary>
    /// Get the first metadata entity of an item - or return a fake one instead
    /// </summary>
    /// <remarks>
    /// The metadata varies a bit, depending on whether it's typed or dynamic.
    /// </remarks>
    internal ITypedMetadata CreateMetadataTyped(string key, string title, Action<IMetadata>? mdInit = null)
        => Cdf.MetadataTyped(PrepareUnderlyingMetadata(key, title, mdInit));

    /// <summary>
    /// Same for dynamic metadata, but it will be treated as just an object, since the code will be in dynamic mode.
    /// </summary>
    internal object CreateMetadataDynamic(string key, string title, Action<IMetadata>? mdInit = null)
        => Cdf.MetadataDynamic(PrepareUnderlyingMetadata(key, title, mdInit));

    private IMetadata PrepareUnderlyingMetadata(string key, string title, Action<IMetadata>? mdInit)
    {
        var mdOf = AppReader.Metadata.GetMetadataOf(TargetTypes.CmsItem, key, title: title);
        mdInit?.Invoke(mdOf);
        return mdOf;
    }

    #endregion
}