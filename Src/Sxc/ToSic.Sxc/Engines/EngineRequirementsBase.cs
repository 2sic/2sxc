using ToSic.Eav.Code.Help;
using ToSic.Eav.Internal.Requirements;
using ToSic.Eav.SysData;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Engines;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class EngineRequirementsBase(IRequirementsService requirements, string logName) : ServiceBase(logName, connect: [requirements])
{
    protected static RenderEngineResult BuildRenderEngineResult(List<RequirementStatus> reqStatus)
    {
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
        return result;
    }

    private static CodeHelp ErrHelpRequirementsNotMet = new(name: "Requirement Not Met", detect: "",
        linkCode: "err-view-config-missing", uiMessage: "Important Requirements not Met");}