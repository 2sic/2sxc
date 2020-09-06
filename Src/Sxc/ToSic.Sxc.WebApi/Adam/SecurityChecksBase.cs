using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.WebApi.Errors;

namespace ToSic.Sxc.WebApi.Adam
{
    public abstract class SecurityChecksBase: HasLog
    {
        #region DI / Constructor

        protected SecurityChecksBase() : base("Sxc.TnScCk") { }

        internal SecurityChecksBase Init(AdamState adamState, bool usePortalRoot, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var callLog = Log.Call<SecurityChecksBase>();
            AdamState = adamState;

            var firstChecker = AdamState.Permissions.PermissionCheckers.First().Value;
            var userMayAdminSomeFiles = firstChecker.UserMay(GrantSets.WritePublished);
            var userMayAdminSiteFiles = firstChecker.GrantedBecause == Conditions.EnvironmentGlobal ||
                                        firstChecker.GrantedBecause == Conditions.EnvironmentInstance;

            UserIsRestricted = !(usePortalRoot
                ? userMayAdminSiteFiles
                : userMayAdminSomeFiles);

            Log.Add($"adminSome:{userMayAdminSomeFiles}, restricted:{UserIsRestricted}");

            return callLog(null, this);
        }

        internal AdamState AdamState;
        public bool UserIsRestricted;

        #endregion

        #region Abstract methods to re-implement

        internal abstract bool TenantAllowsExtension(string fileName);

        #endregion

        internal bool ExtensionIsOk(string fileName, out HttpExceptionAbstraction preparedException)
        {
            if (!TenantAllowsExtension(fileName))
            {
                preparedException = HttpException.NotAllowedFileType(fileName, "Not in whitelisted CMS file types.");
                return false;
            }

            if (SecurityCheckHelpers.IsKnownRiskyExtension(fileName))
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
            Log.Add($"check if user is restricted ({UserIsRestricted}) or if the item '{guid}' is draft");
            exp = null;
            // check that if the user should only see drafts, he doesn't see items of normal data
            if (!UserIsRestricted || FieldPermissionOk(GrantSets.ReadPublished)) return true;

            // check if the data is public
            var itm = AdamState.AdamAppContext.AppRuntime.Entities.Get(guid);
            if (!(itm?.IsPublished ?? false)) return true;

            exp = HttpException.PermissionDenied(Log.Add("user is restricted and may not see published, but item exists and is published - not allowed"));
            return false;
        }

        internal bool FileTypeIsOkForThisField(out HttpExceptionAbstraction preparedException)
        {
            var wrapLog = Log.Call<bool>();
            var fieldDef = AdamState.Attribute;
            bool result;
            // check if this field exists and is actually a file-field or a string (wysiwyg) field
            if (fieldDef == null || !(fieldDef.Type != Eav.Constants.DataTypeHyperlink ||
                                      fieldDef.Type != Eav.Constants.DataTypeString))
            {
                preparedException = HttpException.BadRequest("Requested field '" + AdamState.Field + "' type doesn't allow upload");
                Log.Add($"field type:{fieldDef?.Type} - does not allow upload");
                result = false;
            }
            else
            {
                Log.Add($"field type:{fieldDef.Type}");
                preparedException = null;
                result = true;
            }
            return wrapLog(result.ToString(), result);
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
            var fieldPermissions = Factory.Resolve<AppPermissionCheck>().ForAttribute(
                AdamState.Permissions.Context, appIdentity: AdamState.Block.App, attribute: AdamState.Attribute, parentLog: Log);

            return fieldPermissions.UserMay(requiredGrant);
        }

        internal bool SuperUserOrAccessingItemFolder(string path, out HttpExceptionAbstraction preparedException)
        {
            preparedException = null;
            return !UserIsRestricted || SecurityCheckHelpers.DestinationIsInItem(AdamState.Guid, AdamState.Field, path, out preparedException);
        }
    }
}
