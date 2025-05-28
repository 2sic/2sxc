using ToSic.Eav.Metadata;
using ToSic.Sys.Security.Permissions;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Eav.Apps.Internal;

partial class EavApp: IHasPermissions, IAppWithInternal
{
    #region Metadata and Permission Accessors

    /// <inheritdoc />
    public IMetadataOf Metadata { get; private set; }

    /// <summary>
    /// Permissions of this app
    /// </summary>
    public IEnumerable<IPermission> Permissions => Metadata.Permissions;

    #endregion

    #region Settings, Config, Metadata
    protected IEntity AppSettings;
    protected IEntity AppResources;

    /// <summary>
    /// Assign all kinds of metadata / resources / settings (App-Mode only)
    /// </summary>
    protected void InitializeResourcesSettingsAndMetadata()
    {
        var l = Log.Fn();
        var appReader = AppReaderInt;
        var appSpecs = appReader.Specs;
        Metadata = appSpecs.Metadata;

        // Get the content-items describing various aspects of this app
        AppResources = appSpecs.Resources.MetadataItem;
        AppSettings = appSpecs.Settings.MetadataItem;
        // in some cases these things may be null, if the app was created not allowing side-effects
        // This can usually happen when new apps are being created
        l.A($"HasResources: {AppResources != null}, HasSettings: {AppSettings != null}, HasConfiguration: {appSpecs.Configuration?.Entity != null}");

        // resolve some values for easier access
        Name = appSpecs.Name ?? Constants.ErrorAppName;
        Folder = appSpecs.Folder ?? Constants.ErrorAppName;

        l.Done($"Name: {Name}, Folder: {Folder}");
    }
    #endregion

    // 2024-08-21 2dm - commented out now, for 18.01
    //[PrivateApi("kind of slipped into public till 16.09, but only on the object, never on the IApp, so probably never discovered")]
    //public IAppState AppState => AppStateInt;

    protected internal IAppReader AppReaderInt { get; private set; }

    IAppReader IAppWithInternal.AppReader => AppReaderInt;
}