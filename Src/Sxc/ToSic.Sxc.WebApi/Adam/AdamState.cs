using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Adam
{
    public abstract class AdamState: HasLog
    {
        #region Constructor and DI

        protected AdamState(IServiceProvider serviceProvider, string logName) : base(logName ?? "Adm.State")
        {
            ServiceProvider = serviceProvider;
        }
        public readonly IServiceProvider ServiceProvider;
        public AdamSecurityChecksBase Security;
        public MultiPermissionsTypes Permissions;

        /// <summary>
        /// Initializes the object and performs all the initial security checks
        /// </summary>
        public virtual AdamState Init(IContextOfApp context, string contentType, string fieldName, Guid entityGuid, bool usePortalRoot, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var appId = context.AppState.AppId;
            var callLog = Log.Call<AdamState>($"app: {context.AppState.Show()}, field:{fieldName}, guid:{entityGuid}");
            Context = context;

            Permissions = ServiceProvider.Build<MultiPermissionsTypes>()
                .Init(context, context.AppState, contentType, Log);

            // only do checks on field/guid if it's actually accessing that, if it's on the portal root, don't.
            UseSiteRoot = usePortalRoot;
            if (!usePortalRoot)
            {
                ItemField = fieldName;
                ItemGuid = entityGuid;
            }

            Security = ServiceProvider.Build<AdamSecurityChecksBase>().Init(this, usePortalRoot, Log);

            AdamSecurityCheckHelpers.ThrowIfAccessingRootButNotAllowed(usePortalRoot, Security.UserIsRestricted);

            Log.Add("check if feature enabled");
            if (Security.UserIsRestricted && !Eav.Configuration.Features.Enabled(FeaturesForRestrictedUsers))
                throw HttpException.PermissionDenied(
                    $"low-permission users may not access this - {Eav.Configuration.Features.MsgMissingSome(FeaturesForRestrictedUsers)}");

            if (string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(fieldName)) return callLog(null, this);

            Attribute = Definition(appId, contentType, fieldName);
            if (!Security.FileTypeIsOkForThisField(out var exp))
                throw exp;
            return callLog(null, this);
        }

        #endregion


        // Temp
        public abstract AppRuntime AppRuntime { get; }

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
        {
            FeatureIds.PublicUpload,
            FeatureIds.PublicForms
        };


        private IContentTypeAttribute Definition(int appId, string contentType, string fieldName)
        {
            // try to find attribute definition - for later extra security checks
            var type = State.Get(appId).GetContentType(contentType);
            return type[fieldName];
        }

    }
}
