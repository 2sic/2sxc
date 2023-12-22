using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Insights;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.WebApi.Sys.Insights;

internal class InsightsAppCodeBuild(IAppStates appStates) : InsightsProvider(Link)
{
    public static string Link = "ThisAppCodeBuild";

    public override string HtmlBody()
    {
        if (AppId == null)
            return "please add appid to the url parameters";

        var reader = appStates.GetReader(AppId.Value);


        var msg = "";
        msg += Tags.Nl2Br("Some Statistics\n"
                          + "\n"
                          + "\n"
        );

        if (Parameters.TryGetValue("toggle", out var strToggle) && strToggle == "true")
        {
            msg += Tags.Nl2Br("Toggle: " + strToggle);
        }

        msg += "Type: " + Parameters["type"];


        msg += Linker.LinkTo(view: Name, label: "Run", appId: AppId.Value, more: "type=run");

        return msg;


    }
}