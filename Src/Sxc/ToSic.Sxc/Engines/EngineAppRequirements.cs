using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Internal.Requirements;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class EngineAppRequirements: ServiceBase
    {
        private readonly IRequirementsService _requirements;

        public EngineAppRequirements(IRequirementsService requirements) : base("Eng.AppReq")
        {
            ConnectServices(_requirements = requirements);
        }

        internal RenderEngineResult GetMessageForAppRequirements(AppState appState)
        {
            var l = Log.Fn<RenderEngineResult>();
            // 1. Preflight
            // 1.1. make sure we have an App-State
            if (appState == null) return l.ReturnNull("no appState");


            var reqStatus = appState.PiggyBack.GetOrGenerate(appState, "AppRequirementsStatus",
                () => _requirements.UnfulfilledRequirements(appState.Metadata));

            if (reqStatus.SafeAny())
            {
                var exList2 = reqStatus
                .Select(r => new RenderingException(
                        new CodeHelp(ErrHelpRequirementsNotMet, name: r.Aspect.Name, uiMessage: $"Requirement <em>{r.Aspect.Name}</em> is missing ({r.Aspect.NameId})", linkCode: "sysfeats")))
                    .Cast<Exception>()
                    .ToList();

                var aspectMessages = reqStatus
                    .Select(r =>
                        "<li>" +
                        $"<strong>{r.Aspect.Name}</strong> (<code>{r.Aspect.NameId}</code> - {r.Aspect.Description})." +
                        "</li>")
                    .ToList();

                var html = EngineMessages.Box(
                    "<h2>Important Feature Missing</h2>" +
                    "<div>" +
                    "The App is configured to warn you when an required feature/capability is missing. " +
                    "</div>" +
                    "<ol>" +
                    string.Join("", aspectMessages) +
                    "</ol>" +
                    "<div>" +
                    "To install please follow these ➡️ <a href='https://go.2sxc.org/sysfeats' target='_blank'>instructions</a>." +
                    "</div>",
                    EngineMessages.Warning);

                var result = new RenderEngineResult(html, false, new List<IClientAsset>(), null, exList2);
                return l.Return(result, "error");
            }

            return l.ReturnNull("all seems ok");
        }
        

        private static CodeHelp ErrHelpRequirementsNotMet = new(name: "Requirement Not Met", detect: "",
            linkCode: "err-view-config-missing", uiMessage: "Important Requirements not Met");


    }
}
