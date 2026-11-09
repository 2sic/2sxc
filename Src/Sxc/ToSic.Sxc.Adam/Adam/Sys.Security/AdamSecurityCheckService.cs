using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Security.Files;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Adam.Sys.Security;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamSecurityCheckService(LazySvc<PermissionCheckFactory> permCheckFactory, IAdamSecurityCheckEnvironment securityCheckEnvironment)
    : ServiceWithSetup<AdamContext>("Adm.TnScCk", connect: [permCheckFactory]), IAdamSecurityCheckService
{

    #region DI / Constructor

    public override void Setup(AdamContext options)
    {
        var l = Log.Fn();
        base.Setup(options);
        AdamContext = options;

        var firstChecker = options.Permissions.PermissionCheckers.First().Value;
        var (userMayAdminSomeFiles, conditions, _) = firstChecker.UserMay(GrantSets.WritePublished);
        var userMayAdminSiteFiles = conditions is Conditions.EnvironmentGlobal or Conditions.EnvironmentInstance;

        UserIsRestricted = !(options.MyOptions.UsePortalRoot
            ? userMayAdminSiteFiles
            : userMayAdminSomeFiles);

        l.Done($"adminSome:{userMayAdminSomeFiles}, restricted:{UserIsRestricted}");
    }

    public AdamContext AdamContext { get; private set; } = null!;
    public bool UserIsRestricted { get; private set; }

    #endregion

    #region Abstract methods to re-implement

    //public abstract bool SiteAllowsExtension(string fileName);

    public bool CanEditFolder(Eav.Apps.Assets.IAsset? item) => securityCheckEnvironment.CanEditFolder(this, item);

    #endregion

    public bool ExtensionIsNotOk(string fileName, [NotNullWhen(true)] out HttpExceptionAbstraction? preparedException)
    {
        if (!securityCheckEnvironment.SiteAllowsExtension(this, fileName))
        {
            preparedException = HttpException.NotAllowedFileType(fileName, "Not in whitelisted CMS file types.");
            return true;
        }

        if (FileNames.IsKnownRiskyExtension(fileName))
        {
            preparedException = HttpException.NotAllowedFileType(fileName, "This is a known risky file type.");
            return true;
        }
        preparedException = null;
        return false;
    }


    /// <summary>
    /// Returns true if user isn't restricted, or if the restricted user is accessing a draft item
    /// </summary>
    public bool UserIsRestrictedOrItemIsNotDraft(Guid guid, [NotNullWhen(true)] out HttpExceptionAbstraction? exp)
    {
        var l = Log.Fn<bool>($"is restricted ({UserIsRestricted}) or if the item '{guid}' is draft");
        exp = null;
        // check that if the user should only see drafts, he doesn't see items of normal data
        if (!UserIsRestricted || FieldPermissionOk(GrantSets.ReadPublished))
            return l.ReturnFalse("user not restricted / has grants");

        // check if the data is public
        var itm = AdamContext.AppReader.List.GetOne(guid);
        if (!(itm?.IsPublished ?? false))
            return l.ReturnFalse("not draft");

        const string msg = "User is restricted and may not see published, but item exists and is published - not allowed";
        l.A(msg);
        exp = HttpException.PermissionDenied(msg);
        return l.ReturnTrue("conditions not met");
    }

    public bool FieldDoesNotSupportFiles([NotNullWhen(true)] out HttpExceptionAbstraction? preparedException)
    {
        var l = Log.Fn<bool>();
        var fieldDef = AdamContext.Attribute;
        // check if this field exists and is actually a file-field or a string (wysiwyg) field
        if (fieldDef == null || !(fieldDef.Type != ValueTypes.Hyperlink ||
                                  fieldDef.Type != ValueTypes.String))
        {
            preparedException = HttpException.BadRequest("Requested field '" + AdamContext.ItemFieldName + "' type doesn't allow upload");
            return l.ReturnTrue($"field type:{fieldDef?.Type} - does not allow files");
        }

        preparedException = null;
        return l.ReturnFalse($"field type:{fieldDef.Type}");
    }


    public bool UserNotPermittedOnField(List<Grants> requiredPermissions, [NotNullWhen(true)] out HttpExceptionAbstraction? preparedException)
    {
        // check field permissions, but only for non-publish-data
        if (UserIsRestricted && !FieldPermissionOk(requiredPermissions))
        {
            preparedException = HttpException.PermissionDenied("this field is not configured to allow uploads by the current user");
            return true;
        }
        preparedException = null;
        return false;
    }


    /// <summary>
    /// This will check if the field-definition grants additional rights
    /// Should only be called if the user doesn't have full edit-rights
    /// </summary>
    public bool FieldPermissionOk(List<Grants> requiredGrant)
    {
        var fieldPermissions = permCheckFactory.Value
            .For(AdamContext.Permissions.MyOptions.SiteContext, AdamContext.Context.AppReaderRequired, AdamContext.Attribute?.Permissions, "Field");

        return fieldPermissions.UserMay(requiredGrant).Allowed;
    }

    public bool UserIsRestrictedAndAccessingItemOutsideOfFolder(string? path, [NotNullWhen(true)] out HttpExceptionAbstraction? preparedException)
    {
        if (UserIsRestricted)
            return DestinationIsNotInItem(AdamContext.ItemGuid, AdamContext.ItemFieldName, path, out preparedException);

        preparedException = null;
        return false; // not restricted, so no problem
    }

    private static bool DestinationIsNotInItem(Guid guid, string? field, string? path, [NotNullWhen(true)] out HttpExceptionAbstraction? preparedException)
    {
        if (AdamSecurity.PathIsInItemAdam(guid, field, path))
        {
            preparedException = null;
            return false;
        }
        preparedException = HttpException.PermissionDenied("Can't access a resource which is not part of this item.");
        return true;
    }


    public bool MustThrowIfAccessingRootButNotAllowed(bool usePortalRoot, [NotNullWhen(true)] out HttpExceptionAbstraction? preparedException)
    {
        if (usePortalRoot && UserIsRestricted)
        {
            preparedException = HttpException.BadRequest("you may only create draft-data, so file operations outside of ADAM is not allowed");
            return true;
        }

        preparedException = null;
        return false;
    }


}