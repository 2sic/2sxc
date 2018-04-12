using System;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Interfaces
{
    public interface IRenderingHelpers
    {
        IRenderingHelpers Init(SxcInstance sxc, Log parentLog);

        string WrapInContext(string content,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            int instanceId = 0, 
            int contentBlockId = 0, 
            bool includeEditInfos = false, 
            //string moreClasses = null, 
            //string moreAttribs = null,
            string tag = "div",
            bool autoToolbar = false);

        string ContextAttributes(int instanceId, 
            int contentBlockId, 
            bool includeEditInfos,
            bool autoToolbar);

        string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError, bool addMinimalWrapper, bool encodeMessage);

        string UiContextInfos(bool autoToolbars);
    }
}