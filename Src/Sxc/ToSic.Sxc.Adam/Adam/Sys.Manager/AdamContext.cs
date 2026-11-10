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
    Generator<IAdamSecurityCheckService, AdamContext> adamSecGen,
    LazySvc<ISysFeaturesService> featuresSvc,
    Generator<AdamManager, AdamManager.Options> adamManagerLazy,
    Generator<AdamStorageOfSite> siteStorageGen,
    Generator<AdamStorageOfField> fieldStorageGen)
    : ServiceWithSetup<AdamContext.Options>("Adm.Ctx", connect: [typesPermissions, adamSecGen, featuresSvc, adamManagerLazy, siteStorageGen, fieldStorageGen])
{
    #region Constructor and DI

    public record Options(
        IContextOfApp Context,
        string ContentType,
        string FieldName,
        Guid EntityGuid,
        bool UsePortalRoot);

    public IAdamSecurityCheckService Security => field ??= adamSecGen.New(this);

    public MultiPermissionsTypes Permissions => field ??= typesPermissions.New(new()
    {
        SiteContext = MyOptions.Context, App = MyOptions.Context.AppReaderRequired,
        ContentTypes = [MyOptions.ContentType]
    });

    public AdamManager AdamManager => field ??= adamManagerLazy.New(new(MyOptions.Context, CompatibilityLevels.CompatibilityLevel10));

    public AdamStorage AdamRoot => field ??= MyOptions.UsePortalRoot
        ? siteStorageGen.New().Init(AdamManager)
        : fieldStorageGen.New().InitItemAndField(MyOptions.EntityGuid, MyOptions.FieldName).Init(AdamManager);

    public IAppReader AppReader => AdamManager.AppReader;

    #endregion

    #region Init

    /// <summary>
    /// Initializes the object and performs all the initial security checks
    /// </summary>
    public override void Setup(Options options)
    {
        base.Setup(options);
        var (context, contentType, fieldName, entityGuid, usePortalRoot) = options;
        var l = Log.Fn($"app: {context.AppReaderRequired.Show()}, field:{fieldName}, guid:{entityGuid}, usePortalRoot: {usePortalRoot}");

        // only do checks on field/guid if it's actually accessing that, if it's on the portal root, don't.
        if (!usePortalRoot)
        {
            ItemFieldName = fieldName;
            ItemGuid = entityGuid;
        }

        if (Security.MustThrowIfAccessingRootButNotAllowed(usePortalRoot, out var exception))
            throw exception;

        l.A("check if feature enabled");
        if (Security.UserIsRestricted && !featuresSvc.Value.IsEnabled(FeaturesForRestrictedUsers))
        {
            var msg = featuresSvc.Value.MsgMissingSome(FeaturesForRestrictedUsers);
            throw HttpException.PermissionDenied(
                $"low-permission users may not access this - {msg}");

        }

        if (string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(fieldName))
        {
            l.Done();
            return;
        }

        if (Security.FieldDoesNotSupportFiles(out var exp))
            throw exp;
        l.Done();
    }

    #endregion

    /// <summary>
    /// Determines if the files come from the root (shared files).
    /// Is false, if they come from the item specific ADAM folder.
    /// </summary>
    public bool UseSiteRoot => MyOptions.UsePortalRoot;

    /// <summary>
    /// The field this state is for. Will be null/empty if UsePortalRoot is true
    /// </summary>
    public string? ItemFieldName;

    /// <summary>
    /// The item guid this state is for. Will be Empty if UsePortalRoot is true.
    /// </summary>
    public Guid ItemGuid;

    internal IContentTypeField? Attribute => _attribute.Get(() => AttributeDefinition(MyOptions));
    private readonly LazyGet<IContentTypeField?> _attribute = new();

    public IContextOfApp Context => field ??= MyOptions.Context;

    public readonly Guid[] FeaturesForRestrictedUsers =
    [
        PublicUploadFiles.Guid,
        PublicEditForm.Guid
    ];


    /// <summary>
    /// try to find fieldDef definition - for later extra security checks
    /// </summary>
    private static IContentTypeField? AttributeDefinition(Options options)
    {
        var type = options.Context.AppReaderRequired.GetContentType(options.ContentType);
        return type[options.FieldName];
    }

}