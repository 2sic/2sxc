﻿using System.Text.Json;
using ToSic.Eav.Serialization.Sys;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Edit.Toolbar.ToolbarButtonDecorator;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

internal class UiValueProcessor: UrlValueProcess
{

    public override NameObjectSet? Process(NameObjectSet? set)
    {
        if (set == null)
            return null;
        // Colors - remove any # like #CCDDFF
        if (set.Name == KeyColor)
            return set.Value is string color && color.HasValue() && color.Contains("#")
                ? new(set, value: color.Replace("#", ""))
                : set;

        // Data: must always be object and base64
        // WIP!
        if (set.Name == KeyData || set.Name == KeyNote)
        {
            if (set.Value == null)
                return set;
            var json = JsonSerializer.Serialize(set.Value, JsonOptions.SafeJsonForHtmlAttributes);
            return new(set, value: $"{Json64Prefix}{Base64.Encode(json)}");
        }

        // All others such as icons - make safe
        return MakeSafe(set);
    }
        
}