﻿using ToSic.Eav.Sys;
using ToSic.Lib.Services;
using ToSic.Sys.Capabilities.SysFeatures;
using ToSic.Sys.Code.Help;
using ToSic.Sys.Requirements;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class EngineRequirementsBase(string logName, object[]? connect = default) : ServiceBase(logName, connect: connect)
{
    protected static RenderEngineResult BuildRenderEngineResult(ICollection<RequirementStatus> reqStatus)
    {
        var exList2 = reqStatus
            .Select(r => new RenderingException(ErrHelpRequirementsNotMet with
                {
                    Name = r.Aspect.Name,
                    UiMessage = $"Requirement <em>{r.Aspect.Name}</em> is missing ({r.Aspect.NameId})",
                    LinkCode = "sysfeats",
                }))
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
            $"To install features follow these instructions ➡️ <a href='{EavConstants.GoUrlSysFeats}' target='_blank'>{EavConstants.GoUrlSysFeats}</a>." +
            "</p>",
            EngineMessages.Warning);

        var result = new RenderEngineResult
        {
            Html = html,
            ActivateJsApi = false,
            Assets = [],
            ErrorCode = null,
            ExceptionsOrNull = exList2,
        };
        return result;
    }

    private static readonly CodeHelp ErrHelpRequirementsNotMet = new()
    {
        Name = "Requirement Not Met",
        Detect = "",
        LinkCode = "err-view-config-missing",
        UiMessage = "Important Requirements not Met"
    };
}