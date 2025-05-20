using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Security.Internal;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Data.Internal;
using static ToSic.Eav.Internal.Features.BuiltInFeatures;

namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// The context of ADAM operations - containing site, app, field, entity-guid etc.
/// </summary>
/// <remarks>
/// It's abstract, because there will be a typed implementation inheriting this
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class AdamContext(AdamContext.MyServices services, string logName, object[] connect)
    : ServiceBase<AdamContext.MyServices>(services, logName ?? "Adm.Ctx", connect: connect)
{
    #region Constructor and DI

    public class MyServices(
        Generator<MultiPermissionsTypes> typesPermissions,
        Generator<IAdamSecurityCheckService> adamSecurityGenerator,
        LazySvc<IEavFeaturesService> featuresSvc)
        : MyServicesBase(connect: [typesPermissions, adamSecurityGenerator, featuresSvc])
    {
        public LazySvc<IEavFeaturesService> FeaturesSvc { get; } = featuresSvc;
        public Generator<IAdamSecurityCheckService> AdamSecurityGenerator { get; } = adamSecurityGenerator;
        public Generator<MultiPermissionsTypes> TypesPermissions { get; } = typesPermissions;
    }

    public IAdamSecurityCheckService Security;
    public MultiPermissionsTypes Permissions;

    public abstract AdamManager AdamManagerSimple { get; }

    #endregion
        
    #region Init

    /// <summary>
    /// Initializes the object and performs all the initial security checks
    /// </summary>
    public abstract AdamContext Init(IContextOfApp context, string contentType, string fieldName, Guid entityGuid, bool usePortalRoot, ICodeDataFactory cdf);

    protected AdamContext InitBase(IContextOfApp context, string contentType, string fieldName, Guid entityGuid, bool usePortalRoot, ICodeDataFactory cdf)
    {
        var l = Log.Fn<AdamContext>($"app: {context.AppReader.Show()}, field:{fieldName}, guid:{entityGuid}");
        Context = context;

        Permissions = Services.TypesPermissions.New()
            .Init(context, context.AppReader, contentType);

        // only do checks on field/guid if it's actually accessing that, if it's on the portal root, don't.
        UseSiteRoot = usePortalRoot;
        if (!usePortalRoot)
        {
            ItemField = fieldName;
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

        Attribute = AttributeDefinition(context.AppReader, contentType, fieldName);
        if (!Security.FileTypeIsOkForThisField(out var exp))
            throw exp;
        return l.Return(this);
    }

    #endregion


    // Temp
    public abstract IAppWorkCtx AppWorkCtx { get; }

    /// <summary>
    /// Determines if the files come from the root (shared files).
    /// Is false, if they come from the item specific ADAM folder.
    /// </summary>
    public bool UseSiteRoot;

    /// <summary>
    /// The field this state is for. Will be null/empty if UsePortalRoot is true
    /// </summary>
    public string ItemField;

    /// <summary>
    /// The item guid this state is for. Will be Empty if UsePortalRoot is true.
    /// </summary>
    public Guid ItemGuid;

    internal IContentTypeAttribute Attribute;

    public IContextOfApp Context;

    public readonly Guid[] FeaturesForRestrictedUsers =
    [
        PublicUploadFiles.Guid,
        PublicEditForm.Guid
    ];


    /// <summary>
    /// try to find attribute definition - for later extra security checks
    /// </summary>
    private static IContentTypeAttribute AttributeDefinition(IAppReadContentTypes appReadContentTypes, string contentType, string fieldName)
    {
        var type = appReadContentTypes.GetContentType(contentType);
        return type[fieldName];
    }

}