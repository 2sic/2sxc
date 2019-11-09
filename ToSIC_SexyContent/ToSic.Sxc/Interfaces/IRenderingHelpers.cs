using System;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using Constants = ToSic.Eav.Constants;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Interfaces
{
    public interface IRenderingHelpers
    {
        IRenderingHelpers Init(ICmsBlock cms, ILog parentLog);

        string WrapInContext(string content,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            int instanceId = 0, 
            int contentBlockId = 0, 
            bool editContext = false, 
            string tag = "div",
            bool autoToolbar = false,
            bool addLineBreaks = true);

        string ContextAttributes(int instanceId, 
            int contentBlockId, 
            bool includeEditInfos,
            bool autoToolbar);

        string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError, bool addMinimalWrapper, bool encodeMessage);

        string UiContextInfos(bool autoToolbars);
    }
}