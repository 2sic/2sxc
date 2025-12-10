using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Render.Sys;

[PrivateApi("Internal only")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IRenderingHelper: IHasLog
{
    IRenderingHelper Init(IBlock block);

    string WrapInContext(string? content,
        NoParamOrder npo = default,
        int instanceId = 0,
        int contentBlockId = 0,
        bool editContext = false,
        bool jsApiContext = false,
        string tag = "div",
        bool addLineBreaks = true,
        string? errorCode = default,
        List<Exception>? exsOrNull = default,
        RenderStatistics? statistics = default
    );

    string? DesignErrorMessage(List<Exception> exs, bool addToEventLog, string? msgVisitors = null, string? additionalInfo = null, bool addContextWrapper = false, bool encodeMessage = true);

    string? DesignError(string msgSuperUser, string? msgVisitors = null, bool addContextWrapper = false,
        bool encodeMessage = true, List<Exception>? exsOrNull = default);

    string? DesignWarningForSuperUserOnly(string warning, bool addContextWrapper = false, bool encodeMessage = true);
}