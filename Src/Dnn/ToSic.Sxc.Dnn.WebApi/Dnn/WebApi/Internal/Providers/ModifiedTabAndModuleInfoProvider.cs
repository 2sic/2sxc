// This is a copy of the StandardTabAndModuleInfoProvider from DNN 7.4.2
// It's modified to work with the PageId instead of TabId
// Note that it will be added to a ConcurrentQueue<ITabAndModuleInfoProvider>
// So it's not meant to replace the existing one, but add another mechanism of finding it
// I hope/assume it will be used if the other one fails.

using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Dnn.Providers;

internal sealed class ModifiedTabAndModuleInfoProvider : ITabAndModuleInfoProvider
{
    private const string ModuleIdKey = ContextConstants.ModuleIdKey; // changed 2dm 2021-10-07
    private const string TabIdKey = ContextConstants.PageIdKey; // changed by 2dm 2020-11-20

    public bool TryFindTabId(HttpRequestMessage request, out int tabId)
    {
        tabId = FindInt(request, TabIdKey);
        return tabId > Null.NullInteger;
    }

    public bool TryFindModuleId(HttpRequestMessage request, out int moduleId)
    {
        moduleId = FindInt(request, ModuleIdKey);
        return moduleId > Null.NullInteger;
    }

    public bool TryFindModuleInfo(HttpRequestMessage request, out ModuleInfo moduleInfo)
    {
        moduleInfo = null;

        if (TryFindTabId(request, out var tabId) && TryFindModuleId(request, out var moduleId))
            moduleInfo = ModuleController.Instance.GetModule(moduleId, tabId, false);

        return moduleInfo != null;
    }

    private static int FindInt(HttpRequestMessage requestMessage, string key)
    {
        string value = null;
        if (requestMessage.Headers.TryGetValues(key, out var values))
        {
            value = values.FirstOrDefault();
        }

        if (string.IsNullOrEmpty(value) && requestMessage.RequestUri != null)
        {
            var queryString = HttpUtility.ParseQueryString(requestMessage.RequestUri.Query);
            value = queryString[key];
        }

        return int.TryParse(value, out var id) ? id : Null.NullInteger;
    }

}