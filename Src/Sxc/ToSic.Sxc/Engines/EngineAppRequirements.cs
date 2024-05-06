using ToSic.Eav.Apps.State;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Internal.Requirements;
using ToSic.Eav.Plumbing;
using ToSic.Eav.SysData;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Engines;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EngineAppRequirements(IRequirementsService requirements)
    : ServiceBase("Eng.AppReq", connect: [requirements])
{
    internal RenderEngineResult GetMessageForAppRequirements(IAppStateInternal appState)
    {
        var l = Log.Fn<RenderEngineResult>();

        // 1. Preflight
        // 1.1. make sure we have an App-State
        if (appState == null) return l.ReturnNull("no appState");

        var reqStatus = appState.GetPiggyBackExpiring("AppRequirementsStatus",
            // take the requirements reported by the app
            () => requirements.UnfulfilledRequirements(appState.Metadata)
                // Merge with the basic requirements for 2sxc 17 to work
                .Concat(requirements.UnfulfilledRequirements(SysFeatureSuggestions.CSharp08.ToListOfOne()).ToList())
                .ToList()
        ).Value;

        if (!reqStatus.SafeAny())
            return l.ReturnNull("all seems ok");

        var exList2 = reqStatus
            .Select(r => new RenderingException(
                new(ErrHelpRequirementsNotMet, name: r.Aspect.Name, uiMessage: $"Requirement <em>{r.Aspect.Name}</em> is missing ({r.Aspect.NameId})", linkCode: "sysfeats")))
            .Cast<Exception>()
            .ToList();

        var aspectMessages = reqStatus
            .Select(r =>
                "<li>" +
                $"<strong>{r.Aspect.Name}</strong> <code>{r.Aspect.NameId}</code> <br> {r.Aspect.Description}." +
                (r.Aspect is SysFeature sysFeat ? $"<br>See ➡️ <a href='{sysFeat.Link}' target='_blank'>{sysFeat.Link}</a>" : "") +
                "</li>")
            .ToList();

        var html = EngineMessages.Box(
            "<h2>Feature Missing</h2>" +
            "<p>" +
            "One or more features are missing. It's either required for 2sxc, or the App is configured to warn you when an required feature/capability is missing. " +
            "</p>" +
            "<ol>" +
            string.Join("", aspectMessages) +
            "</ol>" +
            "<p>" +
            "<br>" +
            $"To install features follow these instructions ➡️ <a href='{Eav.Constants.GoUrlSysFeats}' target='_blank'>{Eav.Constants.GoUrlSysFeats}</a>." +
            "</p>",
            EngineMessages.Warning);

        var result = new RenderEngineResult(html, false, [], null, exList2);
        return l.Return(result, "error");

    }
        

    private static CodeHelp ErrHelpRequirementsNotMet = new(name: "Requirement Not Met", detect: "",
        linkCode: "err-view-config-missing", uiMessage: "Important Requirements not Met");

}