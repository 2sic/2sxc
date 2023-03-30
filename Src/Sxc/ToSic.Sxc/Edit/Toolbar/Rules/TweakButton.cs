using System.Collections.Immutable;
using ToSic.Sxc.Web.Url;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleForEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class TweakButton: ITweakButton
    {
        public IImmutableList<object> UiMerge { get; }
        public IImmutableList<object> ParamsMerge { get; }

        internal TweakButton()
        {
            UiMerge = ImmutableList.Create<object>();
            ParamsMerge = ImmutableList.Create<object>();
        }

        private TweakButton(ITweakButton original, IImmutableList<object> uiMerge = default, IImmutableList<object> paramsMerge = default)
        {
            UiMerge = uiMerge ?? original.UiMerge;
            ParamsMerge = paramsMerge ?? original.ParamsMerge;
        }

        public static ITweakButton NoOp(ITweakButton btn) => btn;

        #region UI

        public ITweakButton Note(
            string note,
            string noParamOrder = Protector,
            string type = default
        )
        {
            Protect(noParamOrder, $"{nameof(type)}");
            return Ui(new { note = new { note, type } });
        }

        public ITweakButton Show(bool show = true) => Ui("show", show.ToString().ToLowerInvariant());

        public ITweakButton Color(string color = default, string noParamOrder = Protector, string background = default,
            string foreground = default)
        {
            Protect(noParamOrder, $"{nameof(background)}, {nameof(foreground)}");
            if (color == default)
            {
                color = background;
                if (foreground != default) color += $",{foreground}";
            }

            return Ui("color", color);
        }

        public ITweakButton Tooltip(string title) => Ui("title", title);

        public ITweakButton Group(string group) => Ui("group", group);

        public ITweakButton Icon(string icon) => Ui(new { icon });

        public ITweakButton Class(string classes) => Ui("class", classes);

        public ITweakButton Ui(object ui) => ui == null ? this : new TweakButton(this, UiMerge.Add(ui));

        public ITweakButton Ui(string name, string value) => Ui($"{name}={value}");

        #endregion

        #region Params

        public ITweakButton Parameters(object value) => value == null ? this : new TweakButton(this, paramsMerge: ParamsMerge.Add(value));
        public ITweakButton Parameters(string name, string value) => Parameters($"{name}={value}");

        public ITweakButton Prefill(object prefill) => prefill == null ? this : Parameters(new ObjectToUrl(PrefixPrefill).Serialize(prefill));

        public ITweakButton Filter(object filter) => filter == null ? this : Parameters(new ObjectToUrl(PrefixFilters).Serialize(filter));
        #endregion
    }
}
