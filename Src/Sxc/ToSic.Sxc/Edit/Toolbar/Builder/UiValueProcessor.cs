using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Edit.Toolbar.ToolbarButtonDecorator;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class UiValueProcessor: UrlValueProcess
    {
        // Marker that a string has an svg content
        private const string SvgTag = "<svg";
        // Some SVGs start with an XML header
        private const string XmlTag = "<?xml";

        public override NameObjectSet Process(NameObjectSet set)
        {
            // Special handling of icon - base64 encode if it's an SVG
            if (set.Name == KeyIcon)
            {
                if (!(set.Value is string icon) || !icon.HasValue()) return set;
                if (icon.StartsWith(SvgTag) || (icon.StartsWith(XmlTag) && icon.Contains(SvgTag)))
                    return new NameObjectSet(set, value: $"{Base64Prefix}{Base64.Encode(icon)}");
                return set;
            }

            // Colors - remove any # like #CCDDFF
            if (set.Name == KeyColor)
            {
                if (!(set.Value is string color) || !color.HasValue()) return set;
                return new NameObjectSet(set, value: color.Replace("#", ""));
            }

            // All others unmodified
            return set;
        }
    }
}
