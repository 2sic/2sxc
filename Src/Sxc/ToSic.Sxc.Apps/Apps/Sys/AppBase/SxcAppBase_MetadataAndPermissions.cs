//using ToSic.Eav.Apps.Sys;
//using ToSic.Eav.Metadata;
//using ToSic.Sys.Security.Permissions;
//

//namespace ToSic.Sxc.Apps.Sys;

//partial class SxcAppBase: IHasPermissions, IAppWithInternal
//{

//    /// <summary>
//    /// Assign all kinds of metadata / resources / settings (App-Mode only)
//    /// </summary>
//    protected void InitializeResourcesSettingsAndMetadata()
//    {
//        var l = Log.Fn();
//        //Metadata = appSpecs.Metadata;

//        // Get the content-items describing various aspects of this app
//        //AppResources = appSpecs.Resources.MetadataItem;
//        //AppSettings = AppSpecs.Settings.MetadataItem;
//        // in some cases these things may be null, if the app was created not allowing side-effects
//        // This can usually happen when new apps are being created
//        l.A($"HasResources: {AppResources != null}, HasSettings: {AppSettings != null}, HasConfiguration: {AppReaderInt.Specs.Configuration?.Entity != null}");

//        // resolve some values for easier access
//        //Name = AppReaderInt.Specs.Name; // ?? KnownAppsConstants.ErrorAppName;
//        //Folder = AppReaderInt.Specs.Folder; // ?? KnownAppsConstants.ErrorAppName;

//        l.Done($"Name: {Name}, Folder: {Folder}");
//    }
    

//}