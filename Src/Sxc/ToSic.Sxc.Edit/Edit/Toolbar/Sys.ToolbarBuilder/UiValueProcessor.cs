using System.Text.Json;
using ToSic.Eav.Serialization.Sys;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Edit.Toolbar.Sys.ToolbarButtonDecorator;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

internal class UiValueProcessor: IUrlParameterInGroupOperation
{
    private const string Json64Prefix = "json64:";

    public UrlParameter? Process(UrlParameter? set) =>
        set?.Value == null
            ? null
            : set.Name switch
            {
                // For Colors - remove any # like #CCDDFF
                KeyColor => set.Value is string color && color.HasValue() && color.Contains("#")
                    ? set with { Value = color.Replace("#", "") }
                    : set,

                // For Data or Notes: must always be an object and base64
                KeyData or KeyNote => set with
                {
                    Value = $"{Json64Prefix}{Base64.Encode(JsonSerializer.Serialize(set.Value, JsonOptions.SafeJsonForHtmlAttributes))}"
                },

                // All others such as icons - make safe
                _ => new UrlValueSafeMaker().Process(set)
            };
}