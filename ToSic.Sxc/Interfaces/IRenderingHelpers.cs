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
            IRenderingHelpers dontUseThis = null, 
            int instanceId = 0, 
            int contentBlockId = 0, 
            bool includeEditInfos = false, 
            string moreClasses = null, 
            string moreAttribs = null);

        string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError, bool addMinimalWrapper, bool encodeMessage);

        string GetClientInfosAll();
    }
}