using System;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Blocks.Output
{
    [PrivateApi("Internal only")]
    public interface IRenderingHelper: IHasLog
    {
        IRenderingHelper Init(IBlock block);

        string WrapInContext(string content,
            string noParamOrder = Eav.Parameters.Protector,
            int instanceId = 0, 
            int contentBlockId = 0, 
            bool editContext = false, 
            string tag = "div",
            bool addLineBreaks = true);

        string ContextAttributes(int instanceId, int contentBlockId, bool includeEditInfos);

        string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError = null, string additionalInfo = null, bool addContextWrapper = false, bool encodeMessage = true);

        string DesignError(string msgSuperUser, string msgVisitors = null, bool addContextWrapper = false,
            bool encodeMessage = true);

        string DesignWarningForSuperUserOnly(string warning, bool addContextWrapper = false, bool encodeMessage = true);
    }
}