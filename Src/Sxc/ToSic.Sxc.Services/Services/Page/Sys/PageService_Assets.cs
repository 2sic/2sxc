﻿using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Render.Sys.Output;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;
using Attribute = ToSic.Razor.Markup.Attribute;

namespace ToSic.Sxc.Services.Page.Sys;

partial class PageService
{
    public Attribute? CspWhitelistAttribute() => CspIsEnabled
        ? Tag.Attr(CspConstants.CspWhitelistAttribute, PageServiceShared.CspEphemeralMarker)
        : null;

    public IRawHtmlString AssetAttributes(NoParamOrder noParamOrder = default, bool optimize = true, int priority = 0, string? position = null, bool whitelist = true)
    {
        var attributes = new List<string>();
        if (optimize)
        {
            var strPriority = priority > 100 ? priority.ToString() : "true";
            var strPos = position != null ? ":" + position : null;
            var optAttr = Tag.Attr(ClientAssetConstants.AssetOptimizationsAttributeName, strPriority + strPos);
            attributes.Add(optAttr.ToString());
        }

        if (whitelist)
        {
            var cspAttr = CspWhitelistAttribute();
            if (cspAttr != null)
                attributes.Add(cspAttr.ToString());
        }

        var result = string.Join(" ", attributes);

        return new RawHtmlString(result);
    }

}