using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
using Feats = ToSic.Eav.Configuration.Features;

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
            if (forms.HasValue || styles.HasValue || context.HasValue || autoToolbar.HasValue)
            {
                var feats = new[] {FeatureIds.PublicForms};
                if (!Feats.EnabledOrException(feats, "public forms not available", out var exp))
                    throw exp;
            }

            // find the root host, as this is the one we must tell what js etc. we need
            var hostWithInternals = (BlockBuilder) Block.BlockBuilder.RootBuilder;

            if (js.HasValue || api.HasValue || forms.HasValue)
                hostWithInternals.UiAddJsApi = (js ?? false) || (api ?? false) || (forms ?? false);

            // only update the values if true, otherwise leave untouched
            if (api.HasValue || forms.HasValue)
                hostWithInternals.UiAddEditApi = (api ?? false) || (forms ?? false);

            if (styles.HasValue)
                hostWithInternals.UiAddEditUi = styles.Value;

            if (context.HasValue)
                hostWithInternals.UiAddEditContext = context.Value;

            if (autoToolbar.HasValue)
                hostWithInternals.UiAutoToolbar = autoToolbar.Value;

            return null;
        }

        #endregion Scripts and CSS includes
    }
}