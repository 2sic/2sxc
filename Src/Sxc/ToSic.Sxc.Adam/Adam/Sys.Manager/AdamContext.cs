using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Permissions;
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
public class AdamContext(
    Generator<MultiPermissionsTypes, MultiPermissionsTypes.Options> typesPermissions,
    Generator<IAdamSecurityCheckService> adamSecurityGenerator,
    LazySvc<ISysFeaturesService> featuresSvc,
    LazySvc<AdamManager> adamManagerLazy,
    Generator<AdamStorageOfSite> siteStorageGen,
    Generator<AdamStorageOfField> fieldStorageGen)
    : ServiceWithSetup<AdamContext.Options>("Adm.Ctx", connect: [typesPermissions, adamSecurityGenerator, featuresSvc, adamManagerLazy, siteStorageGen, fieldStorageGen])
{
    #region Constructor and DI

    public record Options(
        IContextOfApp Context,
        string ContentType,
        string FieldName,
        Guid EntityGuid,
        bool UsePortalRoot);

    public IAdamSecurityCheckService Security { get; private set; } = null!;
    public MultiPermissionsTypes Permissions { get; private set; } = null!;

    public AdamManager AdamManager => adamManagerLazy.Value;

    public AdamStorage AdamRoot { get; private set; } = null!;

    public IAppReader AppReader => AdamManager.AppReader;

    #endregion

    #region Init

    /// <summary>
    /// Initializes the object and performs all the initial security checks
    /// </summary>
    public override void Setup(Options options)
    {
        var (context, contentType, fieldName, entityGuid, usePortalRoot) = options;
        var l = Log.Fn($"app: {context.AppReaderRequired.Show()}, field:{fieldName}, guid:{entityGuid}, usePortalRoot: {usePortalRoot}");
        AdamManager.Init(context, CompatibilityLevels.CompatibilityLevel10);
        AdamRoot = usePortalRoot
            ? siteStorageGen.New()
            : fieldStorageGen.New().InitItemAndField(entityGuid, fieldName);
        AdamRoot.Init(AdamManager);

        Context = context;

        Permissions = typesPermissions.New(new()
                { SiteContext = context, App = context.AppReaderRequired, ContentTypes = [contentType] })
            ;
        // only do checks on field/guid if it's actually accessing that, if it's on the portal root, don't.
        UseSiteRoot = usePortalRoot;
        if (!usePortalRoot)
        {
            ItemFieldName = fieldName;
            ItemGuid = entityGuid;
        }

        Security = adamSecurityGenerator.New().Init(this, usePortalRoot);

        if (Security.MustThrowIfAccessingRootButNotAllowed(usePortalRoot, out var exception))
            throw exception;

        l.A("check if feature enabled");
        var sysFeatures = featuresSvc.Value;
        if (Security.UserIsRestricted && !sysFeatures.IsEnabled(FeaturesForRestrictedUsers))
        {
            var msg = sysFeatures.MsgMissingSome(FeaturesForRestrictedUsers);
            throw HttpException.PermissionDenied(
                $"low-permission users may not access this - {msg}");

        }

        if (string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(fieldName))
        {
            l.Done();
            return;
        }

        Attribute = AttributeDefinition(context.AppReaderRequired, contentType, fieldName);
        if (Security.FieldDoesNotSupportFiles(out var exp))
            throw exp;
        l.Done();
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

    internal IContentTypeField? Attribute;

    public IContextOfApp Context { get; private set; } = null!;

    public readonly Guid[] FeaturesForRestrictedUsers =
    [
        PublicUploadFiles.Guid,
        PublicEditForm.Guid
    ];


    /// <summary>
    /// try to find fieldDef definition - for later extra security checks
    /// </summary>
    private static IContentTypeField? AttributeDefinition(IAppReadContentTypes appReadContentTypes, string contentType, string fieldName)
    {
        var type = appReadContentTypes.GetContentType(contentType);
        return type[fieldName];
    }

}