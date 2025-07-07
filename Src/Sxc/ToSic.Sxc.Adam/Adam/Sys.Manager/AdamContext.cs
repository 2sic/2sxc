using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Apps.Sys.Work;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Adam.Sys.Security;
using ToSic.Sxc.Adam.Sys.Storage;
using ToSic.Sxc.Code.Sys;
using ToSic.Sys.Capabilities.Features;
using static ToSic.Sys.Capabilities.Features.BuiltInFeatures;

namespace ToSic.Sxc.Adam.Sys.Manager;

/// <summary>
/// The security context of ADAM operations - containing site, app, field, entity-guid etc.
/// Will check if operations are allowed at setup, and throw errors otherwise.
/// </summary>
/// <remarks>
/// It's abstract, because there will be a typed implementation inheriting this
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamContext(AdamContext.Dependencies services)
    : ServiceBase<AdamContext.Dependencies>(services, "Adm.Ctx")
{
    #region Constructor and DI

    public class Dependencies(
        Generator<MultiPermissionsTypes> typesPermissions,
        Generator<IAdamSecurityCheckService> adamSecurityGenerator,
        LazySvc<ISysFeaturesService> featuresSvc,
        LazySvc<AdamManager> adamManagerLazy,
        Generator<AdamStorageOfSite> siteStorageGen,
        Generator<AdamStorageOfField> fieldStorageGen)
        : DependenciesBase(connect: [typesPermissions, adamSecurityGenerator, featuresSvc, adamManagerLazy, siteStorageGen, fieldStorageGen])
    {
        public LazySvc<ISysFeaturesService> FeaturesSvc { get; } = featuresSvc;
        public LazySvc<AdamManager> AdamManagerLazy { get; } = adamManagerLazy;
        public Generator<AdamStorageOfSite> SiteStorageGen { get; } = siteStorageGen;
        public Generator<AdamStorageOfField> FieldStorageGen { get; } = fieldStorageGen;
        public Generator<IAdamSecurityCheckService> AdamSecurityGenerator { get; } = adamSecurityGenerator;
        public Generator<MultiPermissionsTypes> TypesPermissions { get; } = typesPermissions;
    }

    public IAdamSecurityCheckService Security { get; private set; } = null!;
    public MultiPermissionsTypes Permissions { get; private set; } = null!;

    public AdamManager AdamManager => Services.AdamManagerLazy.Value;

    public AdamStorage AdamRoot { get; private set; } = null!;

    public IAppWorkCtx AppWorkCtx => AdamManager.AppWorkCtx;

    #endregion

    #region Init

    /// <summary>
    /// Initializes the object and performs all the initial security checks
    /// </summary>
    public AdamContext Init(IContextOfApp context, string contentType, string fieldName, Guid entityGuid, bool usePortalRoot)
    {
        var l = Log.Fn<AdamContext>($"app: {context.AppReaderRequired.Show()}, field:{fieldName}, guid:{entityGuid}, usePortalRoot: {usePortalRoot}");
        AdamManager.Init(context, CompatibilityLevels.CompatibilityLevel10);
        AdamRoot = usePortalRoot
            ? Services.SiteStorageGen.New()
            : Services.FieldStorageGen.New().InitItemAndField(entityGuid, fieldName);
        AdamRoot.Init(AdamManager);

        Context = context;

        Permissions = Services.TypesPermissions.New()
            .Init(context, context.AppReaderRequired, contentType);

        // only do checks on field/guid if it's actually accessing that, if it's on the portal root, don't.
        UseSiteRoot = usePortalRoot;
        if (!usePortalRoot)
        {
            ItemFieldName = fieldName;
            ItemGuid = entityGuid;
        }

        Security = Services.AdamSecurityGenerator.New().Init(this, usePortalRoot);

        if (Security.MustThrowIfAccessingRootButNotAllowed(usePortalRoot, out var exception))
            throw exception;

        l.A("check if feature enabled");
        var sysFeatures = Services.FeaturesSvc.Value;
        if (Security.UserIsRestricted && !sysFeatures.IsEnabled(FeaturesForRestrictedUsers))
        {
            var msg = sysFeatures.MsgMissingSome(FeaturesForRestrictedUsers);
            throw HttpException.PermissionDenied(
                $"low-permission users may not access this - {msg}");

        }

        if (string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(fieldName))
            return l.Return(this);

        Attribute = AttributeDefinition(context.AppReaderRequired, contentType, fieldName);
        if (Security.FieldDoesNotSupportFiles(out var exp))
            throw exp;
        return l.Return(this);
    }

    #endregion

    /// <summary>
    /// Determines if the files come from the root (shared files).
    /// Is false, if they come from the item specific ADAM folder.
    /// </summary>
    public bool UseSiteRoot;

    /// <summary>
    /// The field this state is for. Will be null/empty if UsePortalRoot is true
    /// </summary>
    public string? ItemFieldName;

    /// <summary>
    /// The item guid this state is for. Will be Empty if UsePortalRoot is true.
    /// </summary>
    public Guid ItemGuid;

    internal IContentTypeAttribute? Attribute;

    public IContextOfApp Context { get; private set; } = null!;

    public readonly Guid[] FeaturesForRestrictedUsers =
    [
        PublicUploadFiles.Guid,
        PublicEditForm.Guid
    ];


    /// <summary>
    /// try to find attribute definition - for later extra security checks
    /// </summary>
    private static IContentTypeAttribute? AttributeDefinition(IAppReadContentTypes appReadContentTypes, string contentType, string fieldName)
    {
        var type = appReadContentTypes.GetContentType(contentType);
        return type[fieldName];
    }

}