using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.Search;

namespace ToSic.SexyContent.Engines
{
    public interface IEngine
    {
        /// <summary>
        /// Initialize the Engine (pass everything needed for Render to it)
        /// </summary>
        /// <param name="template"></param>
        /// <param name="templatePath"></param>
        /// <param name="app"></param>
        /// <param name="hostingModule"></param>
        /// <param name="localResourcesPath"></param>
        /// <param name="dataSource"></param>
        void Init(Template template, App app, ModuleInfo hostingModule, IDataSource dataSource, InstancePurposes instancePurposes, InstanceContext Sexy);

        /// <summary>
        /// Renders a template, returning a string with the rendered template.
        /// </summary>
        /// <returns></returns>
        string Render();

        void CustomizeData();

        void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, ModuleInfo moduleInfo, DateTime beginDate);

    }
}