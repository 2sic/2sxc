﻿using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Blocks.Output
{
    [PrivateApi("Internal only")]
    public interface IRenderingHelper
    {
        IRenderingHelper Init(IBlock block, ILog parentLog);

        string WrapInContext(string content,
            string noParamOrder = Eav.Parameters.Protector,
            int instanceId = 0, 
            int contentBlockId = 0, 
            bool editContext = false, 
            string tag = "div",
            // 2021-09-01 not used bool autoToolbar = false,
            bool addLineBreaks = true);

        string ContextAttributes(int instanceId, int contentBlockId, bool includeEditInfos);

        string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError, bool addMinimalWrapper, bool encodeMessage);
    }
}