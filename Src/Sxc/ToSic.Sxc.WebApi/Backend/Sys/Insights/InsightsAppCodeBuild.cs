﻿using ToSic.Eav.Sys.Insights;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Backend.Sys;

internal class InsightsAppCodeBuild() : InsightsProvider(new() { Name = Link })
{
    public static string Link = "AppCodeBuild";

    public override string HtmlBody()
    {
        if (AppId == null)
            return "please add appid to the url parameters";

        var msg = "";
        msg += Tags.Nl2Br("Some Statistics\n"
                          + "\n"
                          + "\n"
        );

        if (Parameters.TryGetValue("toggle", out var strToggle) && strToggle?.ToString() == "true")
        {
            msg += Tags.Nl2Br("Toggle: " + strToggle);
        }

        msg += "Type: " + Parameters["type"];


        msg += Linker.LinkTo(view: Specs.Name, label: "Run", appId: AppId.Value, more: "type=run");

        return msg;


    }
}