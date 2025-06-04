using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Services;
using ToSic.Eav.Cms.Internal.Languages;
using ToSic.Eav.Data.PropertyStack.Sys;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Security;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Users.Permissions;
using static ToSic.Eav.Apps.AppStackConstants;


namespace ToSic.Eav.Context.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContextOfApp: ContextOfSite, IContextOfApp
{
    #region Constructor / DI

    /// <summary>
    /// These dependencies are a bit special, because they can be re-used for child context-of...
    /// This is why we gave them a much clearer name, not just the normal "Dependencies"
    /// </summary>
    public new class MyServices(
        ContextOfSite.MyServices siteServices,
        IAppReaderFactory appReaders,
        LazySvc<ISysFeaturesService> features,
        LazySvc<AppUserLanguageCheck> langChecks,
        Generator<IEnvironmentPermission> environmentPermissions,
        LazySvc<AppDataStackService> settingsStack)
        : MyServicesBase<ContextOfSite.MyServices>(siteServices,
            connect: [environmentPermissions, appReaders, features, langChecks, settingsStack])
    {
        public IAppReaderFactory AppReaders { get; } = appReaders;
        public LazySvc<ISysFeaturesService> Features { get; } = features;
        public LazySvc<AppUserLanguageCheck> LangChecks { get; } = langChecks;
        public LazySvc<AppDataStackService> SettingsStack { get; } = settingsStack;
        internal readonly Generator<IEnvironmentPermission> EnvironmentPermissions = environmentPermissions;
    }

    /// <summary>
    /// Constructor for DI
    /// </summary>
    /// <param name="services"></param>
    public ContextOfApp(MyServices services) : this(services, "Sxc.CtxApp", [])
    {
    }
    protected ContextOfApp(MyServices services, string logName, object[] connect) : base(services, logName, connect: connect)
    {
        AppServices = services;
    }
    protected readonly MyServices AppServices;

    #endregion

    public void ResetApp(IAppIdentity appIdentity)
    {
        var l = Log.Fn(appIdentity.Show());
        if (AppIdentity == null || AppIdentity.AppId != appIdentity.AppId)
            AppIdentity = appIdentity;
        l.Done();
    }

    protected virtual IAppIdentity AppIdentity
    {
        get;
        set
        {
            field = value;
            _appReader.Reset();
            _appSettingsStack.Reset();
            _settings.Reset();
            _resources.Reset();
            _userMayEditGet.Reset();
        }
    }

    #region User Permissions / May Edit

    EffectivePermissions ICurrentContextUserPermissions.Permissions => field ??= new(isSiteAdmin: UserMayAdmin, isContentAdmin: UserMayEdit || User.IsContentAdmin);

    private bool UserMayEdit => _userMayEditGet.Get(GetUserMayEdit);
    private readonly GetOnce<bool> _userMayEditGet = new();

    private bool GetUserMayEdit()
    {
        var l = Log.Fn<bool>();
        // Case 1: Superuser always may
        if (User.IsSystemAdmin)
            return l.ReturnTrue("super");

        // Case 2: No App-State
        if (AppReader == null)
        {
            if (UserMayAdmin)
                return l.ReturnTrue("no app, use UserMayAdmin checks");

            // If user isn't allowed yet, it may be that the environment allows it
            var enfPermissions = AppServices.EnvironmentPermissions.New();
            var fromEnv = ((IEnvironmentPermissionSetup)enfPermissions)
                .Init<IContextOfSite>(this, null)
                .EnvironmentAllows(GrantSets.WriteSomething)
                .Allowed;

            return l.Return(fromEnv, "no app, result from Env");
        }

        // Case 3: From App
        var fromApp = Services.AppPermissionCheck.New()
            .ForAppInInstance(this, AppReader)
            .UserMay(GrantSets.WriteSomething)
            .Allowed;

        // Check if language permissions may alter / remove edit permissions
        if (fromApp && AppServices.Features.Value.IsEnabled(BuiltInFeatures.PermissionsByLanguage))
            fromApp = AppServices.LangChecks.Value.UserRestrictedByLanguagePermissions(AppReader) ?? true;

        return l.Return(fromApp, $"{fromApp}");
    }

    #endregion


    public IAppReader AppReader => _appReader.Get(() => AppIdentity == null ? null : AppServices.AppReaders.Get(AppIdentity));
    private readonly GetOnce<IAppReader> _appReader = new();

    #region Settings and Resources

    private AppDataStackService AppDataStackService => _appSettingsStack.Get(() => AppServices.SettingsStack.Value.Init(AppReader));
    private readonly GetOnce<AppDataStackService> _appSettingsStack = new();

    public PropertyStack AppSettings => _settings.Get(() => AppDataStackService.GetStack(RootNameSettings));
    private readonly GetOnce<PropertyStack> _settings = new();
    public PropertyStack AppResources => _resources.Get(() => AppDataStackService.GetStack(RootNameResources));
    private readonly GetOnce<PropertyStack> _resources = new();

    #endregion
}