using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Web.PageFeatures;

// using Feats = ToSic.Eav.Configuration.Features;

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public partial class InPageEditingHelper
    {
        /// <inheritdoc />
        public bool Enabled { 
            get; 
            [PrivateApi("hide, only used for demos")]
            set;
        }

        #region Scripts and CSS includes

        /// <inheritdoc/>
        public string Enable(string noParameterOrder = Eav.Parameters.Protector, bool? js = null, bool? api = null,
            bool? forms = null, bool? context = null, bool? autoToolbar = null, bool? styles = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParameterOrder, "Enable", $"{nameof(js)},{nameof(api)},{nameof(forms)},{nameof(context)},{nameof(autoToolbar)},{nameof(autoToolbar)},{nameof(styles)}");

            // check if feature enabled - if more than the api is needed
            // extend this list if new parameters are added
            // 2021-09-1 2dm- I think this was a bug, it checked for many more things but the feature-form check is only important for the form
            if (forms == true) // .HasValue || styles.HasValue || context.HasValue || autoToolbar.HasValue)
            {
                var feats = new[] {FeatureIds.PublicForms};
                var features = Block.Context.ServiceProvider.Build<Features>();
                if (!/*Feats*/features.EnabledOrException(feats, "public forms not available", out var exp))
                    throw exp;
            }

            // find the root host, as this is the one we must tell what js etc. we need
            var hostWithInternals = (BlockBuilder) Block.BlockBuilder.RootBuilder;

            var psf = Block?.Context?.PageServiceShared;

            if (js == true || api ==true || forms == true)
            {
                psf?.Activate(BuiltInFeatures.Core.Key);
                //hostWithInternals.UiAddJsApi = (js ?? false) || (api ?? false) || (forms ?? false);
            }

            // only update the values if true, otherwise leave untouched
            if (api == true || forms == true)
            {
                psf?.Activate(BuiltInFeatures.EditApi.Key);
                //hostWithInternals.UiAddEditApi = (api ?? false) || (forms ?? false);
            }

            if (styles.HasValue)
            {
                psf?.Activate(BuiltInFeatures.EditUi.Key);
                //hostWithInternals.UiAddEditUi = styles.Value;
            }

            if (context.HasValue)
                hostWithInternals.UiAddEditContext = context.Value;

            if (autoToolbar.HasValue)
            {
                psf?.Activate(BuiltInFeatures.AutoToolbarGlobal.Key);
                hostWithInternals.UiAutoToolbar = autoToolbar.Value;
            }

            return null;
        }

        #endregion Scripts and CSS includes
    }
}