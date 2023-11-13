using System.Collections.Generic;
using System.Collections.Immutable;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleForEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class TweakButton: ITweakButton
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

        /// <summary>
        /// Helper to create an empty TweakButton function as fallback when not provided.
        /// </summary>
        /// <param name="btn"></param>
        /// <returns></returns>
        public static ITweakButton NoOp(ITweakButton btn) => btn;

        #region UI

        public ITweakButton Note(
            string note,
            string noParamOrder = Protector,
            string type = default,
            string background = default
        )
        {
            Protect(noParamOrder, $"{nameof(type)}");
            var noteProps = new Dictionary<string, object> { [nameof(note)] = note };
            if (type != default) noteProps[nameof(type)] = type;
            if (background != default) noteProps[nameof(background)] = background;
            return Ui(new { note = noteProps });
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

            // Remove hashes, because they break the URL api, and spaces, as they shouldn't be there
            color = color?.Replace("#", "").Replace(" ", "");

            return Ui("color", color);
        }

        public ITweakButton Tooltip(string value) => value.IsEmptyOrWs() ? this : Ui("title", value);

        public ITweakButton Group(string value) => value.IsEmptyOrWs() ? this : Ui("group", value);

        public ITweakButton Icon(string value) => value.IsEmptyOrWs() ? this : Ui(new { icon = value });

        public ITweakButton Classes(string value) => value.IsEmptyOrWs() ? this : Ui("class", value);

        public ITweakButton Position(int value) => Ui("pos", value);

        public ITweakButton Ui(object value) => value == null ? this : new TweakButton(this, UiMerge.Add(value));

        public ITweakButton Ui(string name, object value) => (value ?? name) == null ? this : Ui($"{name}={value}");

        #endregion

        #region Params

        public ITweakButton FormParameters(object value) => value == null ? this : Parameters(new ObjectToUrl().SerializeChild(value, PrefixForm));

        public ITweakButton FormParameters(string name, object value) => (value ?? name) == null ? this : FormParameters($"{name}={value}");

        public ITweakButton Parameters(object value) => value == null ? this : new TweakButton(this, paramsMerge: ParamsMerge.Add(value));
        public ITweakButton Parameters(string name, object value) => (value ?? name) == null ? this : Parameters($"{name}={value}");

        public ITweakButton Prefill(object value) => value == null ? this : Parameters(new ObjectToUrl().SerializeChild(value, PrefixPrefill));

        public ITweakButton Prefill(string name, object value) => (value ?? name) == null ? this : Prefill($"{name}={value}");

        public ITweakButton Filter(object value) => value == null ? this : Parameters(new ObjectToUrl().SerializeChild(value, PrefixFilters));
        public ITweakButton Filter(string name, object value) => (value ?? name) == null ? this : Filter($"{name}={value}");

        #endregion
    }
}
