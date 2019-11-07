using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources;
using ToSic.Eav.Logging;
using ToSic.SexyContent.Search;

namespace ToSic.SexyContent.Engines
{
    public interface IEngine
    {
        /// <summary>
        /// Initialize the Engine (pass everything needed for Render to it)
        /// </summary>
        void Init(Template template, App app, IInstanceInfo hostingModule, IDataSource dataSource, InstancePurposes instancePurposes, SxcInstance sxcInstance, ILog parentLog);

        /// <summary>
        /// Renders a template, returning a string with the rendered template.
        /// </summary>
        /// <returns></returns>
        string Render();

        void CustomizeData();

        void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, IInstanceInfo moduleInfo, DateTime beginDate);

        RenderStatusType PreRenderStatus { get; }
    }
}