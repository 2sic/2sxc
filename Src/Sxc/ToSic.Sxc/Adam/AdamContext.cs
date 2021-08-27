using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The context of ADAM operations - containing site, app, field, entity-guid etc.
    /// </summary>
    /// <remarks>
    /// It's abstract, because there will be a typed implementation inheriting this
    /// </remarks>
    public abstract class AdamContext: HasLog
    {
        #region Constructor and DI

        protected AdamContext(IServiceProvider serviceProvider, string logName) : base(logName ?? "Adm.Ctx")
        {
            ServiceProvider = serviceProvider;
        }
        public readonly IServiceProvider ServiceProvider;
        public AdamSecurityChecksBase Security;
        public MultiPermissionsTypes Permissions;

        #endregion
        
        #region Init
        
        /// <summary>
        /// Initializes the object and performs all the initial security checks
        /// </summary>
        public virtual AdamContext Init(IContextOfApp context, string contentType, string fieldName, Guid entityGuid, bool usePortalRoot, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var appId = context.AppState.AppId;
            var callLog = Log.Call<AdamContext>($"app: {context.AppState.Show()}, field:{fieldName}, guid:{entityGuid}");
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

            if (Security.MustThrowIfAccessingRootButNotAllowed(usePortalRoot, out var exception))
                throw exception;

            Log.Add("check if feature enabled");
            if (Security.UserIsRestricted && !Eav.Configuration.Features.Enabled(FeaturesForRestrictedUsers))
            {
                var msg = ServiceProvider.Build<Features>().MsgMissingSome(FeaturesForRestrictedUsers); // Eav.Configuration.Features.MsgMissingSome(FeaturesForRestrictedUsers)
                throw HttpException.PermissionDenied(
                    $"low-permission users may not access this - {msg}");

            }

            if (string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(fieldName)) return callLog(null, this);

            Attribute = AttributeDefinition(appId, contentType, fieldName);
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


        /// <summary>
        /// try to find attribute definition - for later extra security checks
        /// </summary>
        private IContentTypeAttribute AttributeDefinition(int appId, string contentType, string fieldName)
        {
            var type = State.Get(appId).GetContentType(contentType);
            return type[fieldName];
        }

    }
}
