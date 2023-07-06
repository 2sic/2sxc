// This is a copy of the StandardTabAndModuleInfoProvider from DNN 7.4.2
// It's modified to work with the PageId instead of TabId
// Note that it will be added to a ConcurrentQueue<ITabAndModuleInfoProvider>
// So it's not meant to replace the existing one, but add another mechanism of finding it
// I hope/assume it will be used if the other one fails.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Dnn.Providers
{

    public sealed class ModifiedTabAndModuleInfoProvider : ITabAndModuleInfoProvider
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

            int tabId, moduleId;
            if (TryFindTabId(request, out tabId) && TryFindModuleId(request, out moduleId))
            {
                moduleInfo = ModuleController.Instance.GetModule(moduleId, tabId, false);
            }

            return moduleInfo != null;
        }

        private static int FindInt(HttpRequestMessage requestMessage, string key)
        {
            IEnumerable<string> values;
            string value = null;
            if (requestMessage.Headers.TryGetValues(key, out values))
            {
                value = values.FirstOrDefault();
            }

            if (String.IsNullOrEmpty(value) && requestMessage.RequestUri != null)
            {
                var queryString = HttpUtility.ParseQueryString(requestMessage.RequestUri.Query);
                value = queryString[key];
            }

            int id;
            if (Int32.TryParse(value, out id))
            {
                return id;
            }

            return Null.NullInteger;
        }

    }
}
