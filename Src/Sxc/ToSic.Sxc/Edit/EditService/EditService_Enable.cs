using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Web.PageFeatures;
using static ToSic.Eav.Configuration.FeaturesBuiltIn;

namespace ToSic.Sxc.Edit.EditService
{
    public partial class EditService
    {
        /// <inheritdoc />
        public bool Enabled { 
            get; 
            [PrivateApi("hide, only used for demos")]
            set;
        }

        #region Scripts and CSS includes

        /// <inheritdoc/>
        public string Enable(string noParamOrder = Eav.Parameters.Protector, bool? js = null, bool? api = null,
            bool? forms = null, bool? context = null, bool? autoToolbar = null, bool? styles = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "Enable", $"{nameof(js)},{nameof(api)},{nameof(forms)},{nameof(context)},{nameof(autoToolbar)},{nameof(autoToolbar)},{nameof(styles)}");

            // check if feature enabled - if more than the api is needed
            // extend this list if new parameters are added
            if (forms == true)
            {
                var feats = new[] { PublicEditForm.Guid };
                var features = Block.Context.Dependencies.FeaturesInternalGenerator.New;
                if (!features.Enabled(feats, "public forms not available", out var exp))
                    throw exp;
            }

            var psf = Block?.Context?.PageServiceShared;

            if (js == true || api ==true || forms == true) psf?.Activate(BuiltInFeatures.JsCore.Key);

            // only update the values if true, otherwise leave untouched
            if (api == true || forms == true) psf?.Activate(BuiltInFeatures.JsCms.Key);

            if (styles.HasValue) psf?.Activate(BuiltInFeatures.Toolbars.Key);

            if (context.HasValue) psf?.Activate(BuiltInFeatures.ModuleContext.Key);

            if (autoToolbar.HasValue) psf?.Activate(BuiltInFeatures.ToolbarsAuto.Key);

            return null;
        }

        #endregion Scripts and CSS includes
    }
}