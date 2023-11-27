using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;
using BuiltInFeatures = ToSic.Sxc.Web.PageFeatures.BuiltInFeatures;

namespace ToSic.Sxc.Edit.EditService;

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
    public string Enable(NoParamOrder noParamOrder = default, bool? js = null, bool? api = null,
        bool? forms = null, bool? context = null, bool? autoToolbar = null, bool? styles = null
    ) => Log.Func<string>(() =>
    {
        //Eav.Parameters.Protect(noParamOrder,
        //    $"{nameof(js)},{nameof(api)},{nameof(forms)},{nameof(context)},{nameof(autoToolbar)},{nameof(autoToolbar)},{nameof(styles)}");

        var ps = _DynCodeRoot.GetKit<ServiceKit14>()?.Page;
        if (ps == null)
            return (null, "page service not found");

        if (js == true || api == true || forms == true) ps.Activate(BuiltInFeatures.JsCore.NameId);

        // only update the values if true, otherwise leave untouched
        // Must activate the "public" one JsCms, not internal, so feature-tests will run
        if (api == true || forms == true) ps.Activate(BuiltInFeatures.JsCms.NameId);

        if (styles == true) ps.Activate(BuiltInFeatures.Toolbars.NameId);

        if (context == true) ps.Activate(BuiltInFeatures.ContextModule.NameId);

        if (autoToolbar == true) ps.Activate(BuiltInFeatures.ToolbarsAuto.NameId);

        return (null, "ok");
    });

    #endregion Scripts and CSS includes
}