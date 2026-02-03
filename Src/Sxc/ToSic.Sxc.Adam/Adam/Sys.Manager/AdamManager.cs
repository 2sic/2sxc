using ToSic.Eav.Apps.Sys.Work;
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
public class AdamManager(AdamManager.Dependencies services)
    : ServiceBase<AdamManager.Dependencies>(services, "Adm.Managr")
{
    #region Dependencies

    public record Dependencies(
        LazySvc<ICodeDataFactory> CdfIfNotProvided,
        AdamConfiguration AdamConfiguration,
        LazySvc<IAdamFileSystem> AdamFsLazy,
        Generator<AdamStorageOfField> FieldStorageGenerator,
        AdamGenericHelper AdamGenericHelper)
        : DependenciesRecord(connect: [CdfIfNotProvided, AdamConfiguration, AdamFsLazy, FieldStorageGenerator]);

    #endregion

    #region Init

    public AdamManager Init(IContextOfApp appCtx, int compatibility, ICodeDataFactory? cdf = default)
    {
        var l = Log.Fn<AdamManager>();
        AppContext = appCtx;
        Cdf = cdf
              ?? Services.CdfIfNotProvided
                  .SetInit(obj => obj.SetFallbacks(appCtx.Site, compatibility, this))
                  .Value;
        return l.Return(this, "ready");
    }

    public bool UseTypedAssets => Cdf.CompatibilityLevel >= CompatibilityLevels.MinLevelForTyped;

    [field: AllowNull, MaybeNull]
    public IAdamFileSystem AdamFs => field ??= Services.AdamFsLazy.SetInit(f => f.Init(this)).Value;

    [field: AllowNull, MaybeNull]
    public IAppWorkCtx AppWorkCtx => field ??= new AppWorkCtx(AppContext.AppReaderRequired);

    private IContextOfApp AppContext { get; set; } = null!;

    [field: AllowNull, MaybeNull]
    public ISite Site => field ??= AppContext.Site;

    internal ICodeDataFactory Cdf { get; private set; } = null!;

    #endregion

    /// <summary>
    /// Path to the app assets
    /// </summary>
    [field: AllowNull, MaybeNull]
    public string Path => field ??= Services.AdamConfiguration.PathForApp(AppContext.AppReaderRequired.Specs);

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
        var folderStorage = Services.FieldStorageGenerator.New().InitItemAndField(entityGuid, fieldName);
        folderStorage.Init(this);
        var folder = Services.AdamGenericHelper.FolderOfField(this, folderStorage, field);
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
        var mdOf = AppWorkCtx.AppReader.Metadata.GetMetadataOf(TargetTypes.CmsItem, key, title: title);
        mdInit?.Invoke(mdOf);
        return mdOf;
    }

    #endregion
}