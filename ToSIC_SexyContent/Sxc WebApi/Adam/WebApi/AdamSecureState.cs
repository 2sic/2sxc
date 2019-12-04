using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Security;
using ToSic.Sxc.WebApi;
using SysConf = ToSic.Eav.Configuration;
using Feats = ToSic.Eav.Configuration.Features;
using IApp = ToSic.Sxc.Apps.IApp;
using ICmsBlock = ToSic.Sxc.Blocks.ICmsBlock;

namespace ToSic.Sxc.Adam.WebApi
{
    internal class AdamSecureState: MultiPermissionsTypes
    {
        public string Field;
        public bool UserIsRestricted;
        public bool UserMayAdminSiteFiles;
        public Guid Guid;
        internal ContainerBase ContainerContext;
        internal AdamAppContext AdamAppContext;
        internal IContentTypeAttribute Attribute;

        public readonly Guid[] FeaturesForRestrictedUsers =
            {
                SysConf.FeatureIds.PublicUpload,
                SysConf.FeatureIds.PublicForms
            };

        /// <summary>
        /// Initializes the object and performs all the initial security checks
        /// </summary>
        public AdamSecureState(ICmsBlock cmsInstance, int appId, string contentType, string field, Guid guid, bool usePortalRoot, ILog log)
            : base(cmsInstance, appId, contentType, log)
        {
            // only do checks on field/guid if it's actually accessing that, if it's on the portal root, don't.
            if (!usePortalRoot)
            {
                Field = field;
                Guid = guid;
            }

            var firstChecker = PermissionCheckers.First().Value;
            var userMayAdminSomeFiles = firstChecker.UserMay(GrantSets.WritePublished);
            UserMayAdminSiteFiles = firstChecker.GrantedBecause == Conditions.EnvironmentGlobal ||
                                   firstChecker.GrantedBecause == Conditions.EnvironmentInstance;

            UserIsRestricted = !(usePortalRoot
                ? UserMayAdminSiteFiles
                : userMayAdminSomeFiles);


            Log.Add($"AdamSecureState - field:{field}, guid:{guid}, adminSome:{userMayAdminSomeFiles}, restricted:{UserIsRestricted}");

            SecurityChecks.ThrowIfAccessingRootButNotAllowed(usePortalRoot, UserIsRestricted);

            Log.Add("check if feature enabled");
            if (UserIsRestricted && !Feats.Enabled(FeaturesForRestrictedUsers))
                throw Http.PermissionDenied(
                    $"low-permission users may not access this - {Feats.MsgMissingSome(FeaturesForRestrictedUsers)}");

            PrepCore(App, guid, field, usePortalRoot);

            if (string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(field)) return;

            Attribute = Definition(appId, contentType, field);
            if(!FileTypeIsOkForThisField(out var exp))
                throw exp;
        }

        private void PrepCore(IApp app, Guid entityGuid, string fieldName, bool usePortalRoot)
        {
            Log.Add("PrepCore(...)");
            var dnn = new DnnHelper(CmsInstance?.Container);
            var tenant = new Tenant(dnn.Portal);
            AdamAppContext = new AdamAppContext(tenant, app, CmsInstance, Log);
            ContainerContext = usePortalRoot
                ? new ContainerOfTenant(AdamAppContext) as ContainerBase
                : new ContainerOfField(AdamAppContext, entityGuid, fieldName);
        }

        /// <summary>
        /// Returns true if user isn't restricted, or if the retricted user is accessing a draft item
        /// </summary>
        internal bool UserIsNotRestrictedOrItemIsDraft(Guid guid, out HttpResponseException exp)
        {
            Log.Add($"check if user is restricted ({UserIsRestricted}) or if the item '{guid}' is draft");
            exp = null;
            // check that if the user should only see drafts, he doesn't see items of normal data
            if (!UserIsRestricted || FieldPermissionOk(GrantSets.ReadPublished)) return true;

            // check if the data is public
            var itm = AdamAppContext.AppRuntime.Entities.Get(guid);
            if (!(itm?.IsPublished ?? false)) return true;

            exp = Http.PermissionDenied(Log.Add("user is restricted and may not see published, but item exists and is published - not allowed"));
            return false;
        }

        private IContentTypeAttribute Definition(int appId, string contentType, string fieldName)
        {
            // try to find attribute definition - for later extra security checks
            var appRead = new AppRuntime(appId, Log);
            var type = appRead.ContentTypes.Get(contentType);
            return type[fieldName];
        }

        public bool FileTypeIsOkForThisField(out HttpResponseException preparedException)
        {
            var wrapLog = Log.Call<bool>("FileTypeIsOkForThisField");
            var fieldDef = Attribute;
            bool result;
            // check if this field exists and is actually a file-field or a string (wysiwyg) field
            if (fieldDef == null || !(fieldDef.Type != Eav.Constants.DataTypeHyperlink ||
                                      fieldDef.Type != Eav.Constants.DataTypeString))
            {
                preparedException = Http.BadRequest("Requested field '" + Field + "' type doesn't allow upload");
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

        public bool UserIsPermittedOnField(List<Grants> requiredPermissions, out HttpResponseException preparedException)
        {
            // check field permissions, but only for non-publish-data
            if (UserIsRestricted && !FieldPermissionOk(requiredPermissions))
            {
                preparedException = Http.PermissionDenied("this field is not configured to allow uploads by the current user");
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
            var fieldPermissions = new DnnPermissionCheck(Log,
                instance: CmsInstance.Container,
                permissions1: Attribute.Permissions,
                appIdentity: CmsInstance.App);

            return fieldPermissions.UserMay(requiredGrant);
        }

        public bool SuperUserOrAccessingItemFolder(string path, out HttpResponseException preparedException)
        {
            preparedException = null;
            return !UserIsRestricted || SecurityChecks.DestinationIsInItem(Guid, Field, path, out preparedException);
        }

        public bool ExtensionIsOk(string fileName, out HttpResponseException preparedException)
        {
            if (!SecurityChecks.IsAllowedDnnExtension(fileName))
            {
                preparedException = Http.NotAllowedFileType(fileName, "Not in whitelisted CMS file types.");
                return false;
            }

            if (SecurityChecks.IsKnownRiskyExtension(fileName))
            {
                preparedException = Http.NotAllowedFileType(fileName, "This is a known risky file type.");
                return false;
            }
            preparedException = null;
            return true;
        }

        internal bool CustomFileFilterOk(string additionalFilter, string fileName)
        {
            var wrapLog = Log.Call<bool>("CustomFileFilterOk");
            var extension = Path.GetExtension(fileName)?.TrimStart('.') ?? "";
            var hasNonAzChars = new Regex("[^a-z]", RegexOptions.IgnoreCase);

            Log.Add($"found additional file filter: {additionalFilter}");
            var filters = additionalFilter.Split(',').Select(f => f.Trim()).ToList();
            Log.Add($"found {filters.Count} filters in {additionalFilter}, will test on {fileName} with ext {extension}");

            foreach (var f in filters)
            {
                // just a-z characters
                if (!hasNonAzChars.IsMatch(f))
                    if (extension == f)
                        return wrapLog($"filter {f} matched filename {fileName}", true);
                    else
                        continue;

                // could be regex or simple *.ext
                if (f.StartsWith("*."))
                    if (string.Equals(extension, f.Substring(2), StringComparison.OrdinalIgnoreCase))
                        return wrapLog($"filter {f} matched filename {fileName}", true);
                    else
                        continue;

                // it's a regex
                try
                {
                    if (new Regex(f, RegexOptions.IgnoreCase).IsMatch(fileName))
                        return wrapLog($"filter {f} matched filename {fileName}", true);
                }
                catch
                {
                    Log.Add($"filter {f} was detected as reg-ex but threw error");
                }

            }

            return false;
        }

        //protected override Dictionary<string, IPermissionCheck> InitializePermissionChecks() 
        //    => ContentTypes.Any() 
        //        ? InitPermissionChecksForType(ContentTypes) 
        //        : InitPermissionChecksForApp();
    }
}
