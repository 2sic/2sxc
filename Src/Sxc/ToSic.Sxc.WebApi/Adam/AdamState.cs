using System;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.Security;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.Adam
{
    internal class AdamState: HasLog
    {
        public string Field;
        public Guid Guid;
        internal ContainerBase ContainerContext;
        internal AdamAppContext AdamAppContext;
        internal IContentTypeAttribute Attribute;

        internal readonly IBlock Block;

        public readonly Guid[] FeaturesForRestrictedUsers =
        {
            FeatureIds.PublicUpload,
            FeatureIds.PublicForms
        };

        public SecurityChecksBase Security;
        public MultiPermissionsTypes Permissions;

        public IApp App;


        #region Constructor / DI

        /// <summary>
        /// Initializes the object and performs all the initial security checks
        /// </summary>
        public AdamState(IBlock block, int appId, string contentType, string field, Guid guid, bool usePortalRoot, ILog log)
            : base("Adm.State", log)
        {
            var callLog = Log.Call($"field:{field}, guid:{guid}");
            App = Factory.Resolve<Apps.App>().Init(appId, log, block);
            Permissions = new MultiPermissionsTypes()
                .Init(block.Context, App, contentType, Log);
            Block = block;

            // only do checks on field/guid if it's actually accessing that, if it's on the portal root, don't.
            if (!usePortalRoot)
            {
                Field = field;
                Guid = guid;
            }

            Security = Factory.Resolve<SecurityChecksBase>().Init(this, usePortalRoot, Log);
            
            SecurityCheckHelpers.ThrowIfAccessingRootButNotAllowed(usePortalRoot, Security.UserIsRestricted);

            Log.Add("check if feature enabled");
            if (Security.UserIsRestricted && !Features.Enabled(FeaturesForRestrictedUsers))
                throw HttpException.PermissionDenied(
                    $"low-permission users may not access this - {Features.MsgMissingSome(FeaturesForRestrictedUsers)}");

            PrepCore(App, guid, field, usePortalRoot);

            if (string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(field)) return;

            Attribute = Definition(appId, contentType, field);
            if (!Security.FileTypeIsOkForThisField(out var exp))
                throw exp;
            callLog(null);
        }

        #endregion

        #region Initialization methods

        private IContentTypeAttribute Definition(int appId, string contentType, string fieldName)
        {
            // try to find attribute definition - for later extra security checks
            var type = State.Get(appId).GetContentType(contentType);
            return type[fieldName];
        }

        protected void PrepCore(IApp app, Guid entityGuid, string fieldName, bool usePortalRoot)
        {
            Log.Add("PrepCore(...)");
            AdamAppContext = new AdamAppContext(Permissions.Context.Tenant, app, Block, 10, Log);
            ContainerContext = usePortalRoot
                ? new ContainerOfTenant(AdamAppContext) as ContainerBase
                : new ContainerOfField(AdamAppContext, entityGuid, fieldName);
        }
        #endregion
    }
}
