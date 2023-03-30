using System.Collections.Immutable;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class TweakButton: ITweakButton
    {
        internal TweakButton()
        {
            UiMerge = ImmutableList.Create<object>();
        }

        private TweakButton(TweakButton original, IImmutableList<object> uiMerge = default)
        {
            UiMerge = uiMerge ?? original.UiMerge;
        }

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

        public ITweakButton Ui(object ui) => new TweakButton(this, UiMerge.Add(ui));

        public ITweakButton Ui(string name, string value) => new TweakButton(this, UiMerge.Add($"{name}={value}"));

        public IImmutableList<object> UiMerge { get; }
    }
}
