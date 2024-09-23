using ToSic.Eav.Security;
using ToSic.Eav.Security.Files;
using ToSic.Eav.Security.Internal;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Adam.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class AdamSecurityChecksBase(AdamSecurityChecksBase.MyServices services, string logPrefix)
    : ServiceBase<AdamSecurityChecksBase.MyServices>(services, $"{logPrefix}.TnScCk")
{

    #region DI / Constructor

    public class MyServices: MyServicesBase
    {
        public Generator<AppPermissionCheck> AppPermissionChecks { get; }

        public MyServices(Generator<AppPermissionCheck> appPermissionChecks)
        {
            ConnectLogs([
                AppPermissionChecks = appPermissionChecks
            ]);
        }
    }

    internal AdamSecurityChecksBase Init(AdamContext adamContext, bool usePortalRoot)
    {
        var callLog = Log.Fn<AdamSecurityChecksBase>();
        AdamContext = adamContext;

        var firstChecker = AdamContext.Permissions.PermissionCheckers.First().Value;
        var userMayAdminSomeFiles = firstChecker.UserMay(GrantSets.WritePublished);
        var userMayAdminSiteFiles = firstChecker.GrantedBecause == Conditions.EnvironmentGlobal ||
                                    firstChecker.GrantedBecause == Conditions.EnvironmentInstance;

        UserIsRestricted = !(usePortalRoot
            ? userMayAdminSiteFiles
            : userMayAdminSomeFiles);

        Log.A($"adminSome:{userMayAdminSomeFiles}, restricted:{UserIsRestricted}");

        return callLog.Return(this);
    }

    internal AdamContext AdamContext;
    public bool UserIsRestricted;

    #endregion

    #region Abstract methods to re-implement

    public abstract bool SiteAllowsExtension(string fileName);

    public abstract bool CanEditFolder(Eav.Apps.Assets.IAsset item);

    #endregion

    internal bool ExtensionIsOk(string fileName, out HttpExceptionAbstraction preparedException)
    {
        if (!SiteAllowsExtension(fileName))
        {
            preparedException = HttpException.NotAllowedFileType(fileName, "Not in whitelisted CMS file types.");
            return false;
        }

        if (FileNames.IsKnownRiskyExtension(fileName))
        {
            preparedException = HttpException.NotAllowedFileType(fileName, "This is a known risky file type.");
            return false;
        }
        preparedException = null;
        return true;
    }


    /// <summary>
    /// Returns true if user isn't restricted, or if the restricted user is accessing a draft item
    /// </summary>
    internal bool UserIsNotRestrictedOrItemIsDraft(Guid guid, out HttpExceptionAbstraction exp)
    {
        Log.A($"check if user is restricted ({UserIsRestricted}) or if the item '{guid}' is draft");
        exp = null;
        // check that if the user should only see drafts, he doesn't see items of normal data
        if (!UserIsRestricted || FieldPermissionOk(GrantSets.ReadPublished)) return true;

        // check if the data is public
        var itm = AdamContext.AppWorkCtx.AppReader.List.One(guid);
        if (!(itm?.IsPublished ?? false)) return true;

        const string msg = "User is restricted and may not see published, but item exists and is published - not allowed";
        Log.A(msg);
        exp = HttpException.PermissionDenied(msg);
        return false;
    }

    internal bool FileTypeIsOkForThisField(out HttpExceptionAbstraction preparedException)
    {
        var l = Log.Fn<bool>();
        var fieldDef = AdamContext.Attribute;
        bool result;
        // check if this field exists and is actually a file-field or a string (wysiwyg) field
        if (fieldDef == null || !(fieldDef.Type != ValueTypes.Hyperlink ||
                                  fieldDef.Type != ValueTypes.String))
        {
            preparedException = HttpException.BadRequest("Requested field '" + AdamContext.ItemField + "' type doesn't allow upload");
            Log.A($"field type:{fieldDef?.Type} - does not allow upload");
            result = false;
        }
        else
        {
            Log.A($"field type:{fieldDef.Type}");
            preparedException = null;
            result = true;
        }
        return l.ReturnAndLog(result);
    }


    internal bool UserIsPermittedOnField(List<Grants> requiredPermissions, out HttpExceptionAbstraction preparedException)
    {
        // check field permissions, but only for non-publish-data
        if (UserIsRestricted && !FieldPermissionOk(requiredPermissions))
        {
            preparedException = HttpException.PermissionDenied("this field is not configured to allow uploads by the current user");
            return false;
        }
        preparedException = null;
        return true;
    }


    /// <summary>
    /// This will check if the field-definition grants additional rights
    /// Should only be called if the user doesn't have full edit-rights
    /// </summary>
    public bool FieldPermissionOk(List<Grants> requiredGrant)
    {
        var fieldPermissions = Services.AppPermissionChecks.New()
            .ForAttribute(AdamContext.Permissions.Context, AdamContext.Context.AppReader, AdamContext.Attribute);

        return fieldPermissions.UserMay(requiredGrant);
    }

    internal bool SuperUserOrAccessingItemFolder(string path, out HttpExceptionAbstraction preparedException)
    {
        preparedException = null;
        return !UserIsRestricted || DestinationIsInItem(AdamContext.ItemGuid, AdamContext.ItemField, path, out preparedException);
    }

    private static bool DestinationIsInItem(Guid guid, string field, string path, out HttpExceptionAbstraction preparedException)
    {
        var inAdam = Security.PathIsInItemAdam(guid, field, path);
        preparedException = inAdam
            ? null
            : HttpException.PermissionDenied("Can't access a resource which is not part of this item.");
        return inAdam;
    }


    internal bool MustThrowIfAccessingRootButNotAllowed(bool usePortalRoot, out HttpExceptionAbstraction preparedException)
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