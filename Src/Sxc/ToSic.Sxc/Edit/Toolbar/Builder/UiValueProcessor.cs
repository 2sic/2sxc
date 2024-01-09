using System.Text.Json;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Edit.Internal.Toolbar.ToolbarButtonDecorator;

namespace ToSic.Sxc.Edit.Toolbar;

internal class UiValueProcessor: UrlValueProcess
{

    public override NameObjectSet Process(NameObjectSet set)
    {
        // Colors - remove any # like #CCDDFF
        if (set.Name == KeyColor)
            return set.Value is string color && color.HasValue() && color.Contains("#")
                ? new NameObjectSet(set, value: color.Replace("#", ""))
                : set;

        // Data: must always be object and base64
        // WIP!
        if (set.Name == KeyData || set.Name == KeyNote)
        {
            if (set.Value == null) return set;
            var json = JsonSerializer.Serialize(set.Value, JsonOptions.SafeJsonForHtmlAttributes);
            return new NameObjectSet(set, value: $"{Json64Prefix}{Base64.Encode(json)}");
        }

        // All others such as icons - make safe
        return MakeSafe(set);
    }
        
}