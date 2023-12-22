using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Internal.Requirements;
using ToSic.Eav.Plumbing;
using ToSic.Eav.SysData;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EngineAppRequirements: ServiceBase
{
    private readonly IRequirementsService _requirements;

    public EngineAppRequirements(IRequirementsService requirements) : base("Eng.AppReq")
    {
        ConnectServices(_requirements = requirements);
    }

    internal RenderEngineResult GetMessageForAppRequirements(IAppStateInternal appState)
    {
        var l = Log.Fn<RenderEngineResult>();
        // 1. Preflight
        // 1.1. make sure we have an App-State
        if (appState == null) return l.ReturnNull("no appState");


        var reqStatus = appState.PiggyBack.GetOrGenerate(appState, "AppRequirementsStatus",
            // take the requirements reported by the app
            () => _requirements.UnfulfilledRequirements(appState.Metadata)
                // Merge with the basic requirements for 2sxc 17 to work
                .Concat(_requirements.UnfulfilledRequirements(SysFeatureSuggestions.CSharp08.ToListOfOne())
                    .ToList())
        );

        if (reqStatus.SafeAny())
        {
            var rsList = reqStatus.ToList();
            var exList2 = rsList
                .Select(r => new RenderingException(
                    new CodeHelp(ErrHelpRequirementsNotMet, name: r.Aspect.Name, uiMessage: $"Requirement <em>{r.Aspect.Name}</em> is missing ({r.Aspect.NameId})", linkCode: "sysfeats")))
                .Cast<Exception>()
                .ToList();

            var aspectMessages = rsList
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

            var result = new RenderEngineResult(html, false, new List<IClientAsset>(), null, exList2);
            return l.Return(result, "error");
        }

        return l.ReturnNull("all seems ok");
    }
        

    private static CodeHelp ErrHelpRequirementsNotMet = new(name: "Requirement Not Met", detect: "",
        linkCode: "err-view-config-missing", uiMessage: "Important Requirements not Met");


}