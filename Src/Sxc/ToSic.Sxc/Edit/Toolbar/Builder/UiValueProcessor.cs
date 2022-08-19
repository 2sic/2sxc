using Newtonsoft.Json;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Web.Url;
using System.Linq;
using static ToSic.Sxc.Edit.Toolbar.ToolbarButtonDecorator;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class UiValueProcessor: UrlValueProcess
    {
        // Base64 marker for rule encoding
        public static string Base64Prefix = "base64:";
        public static string Json64Prefix = "json64:";

        public static char[] UnsafeChars = {
            '\n', '\r',
            '<', '>',
            '"', '\'',
            '=', '&', '?', '#'
        };

        public override NameObjectSet Process(NameObjectSet set)
        {
            // Colors - remove any # like #CCDDFF
            if (set.Name == KeyColor)
                return set.Value is string color && color.HasValue() && color.Contains("#")
                    ? new NameObjectSet(set, value: color.Replace("#", ""))
                    : set;

            // Data: must always be object and base64
            // WIP!
            if (set.Name == KeyData)
            {
                if (set.Value == null) return set;
                var json = JsonConvert.SerializeObject(set.Value);
                return new NameObjectSet(set, value: $"{Json64Prefix}{Base64.Encode(json)}");
            }

            // All others such as icons - make safe
            return MakeSafe(set);
        }
        
        /// <summary>
        /// Converts any string value which contains unsafe characters to base64
        /// Works for SVG icons and similar
        /// Requires the receiving system (in this case the inpage JS) to handle strings starting with "base64:" differently. 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        private NameObjectSet MakeSafe(NameObjectSet set)
        {
            var obj = set.Value;
            return obj is string str && str.HasValue() && UnsafeChars.Any(c => str.Contains(c))
                ? new NameObjectSet(set, value: $"{Base64Prefix}{Base64.Encode(str)}")
                : set;
        }
    }
}
