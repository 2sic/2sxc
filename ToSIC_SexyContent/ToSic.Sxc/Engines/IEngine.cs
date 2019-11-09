using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.SexyContent;
using ToSic.SexyContent.Search;
using ToSic.Sxc.Views;
using App = ToSic.SexyContent.App;

namespace ToSic.Sxc.Engines
{
    /// <summary>
    /// The sub-system in charge of taking
    /// - a configuration for an instance (aka Module)
    /// - a template
    /// and using all that to produce an html-string for the browser. 
    /// </summary>
    [PublicApi]
    public interface IEngine
    {
        /// <summary>
        /// Initialize the Engine (pass everything needed for Render to it).<br/>
        /// This is not in the constructor, because IEngines usually get constructed with DI,
        /// so the constructor is off-limits. 
        /// </summary>
        [PrivateApi]
        void Init(IView template, App app, IInstanceInfo hostingModule, IDataSource dataSource, InstancePurposes instancePurposes, SxcInstance sxcInstance, ILog parentLog);

        /// <summary>
        /// Renders a template, returning a string with the rendered template.
        /// </summary>
        /// <returns>The string - usually HTML - which the engine created. </returns>
        string Render();

        /// <summary>
        /// Mechanism which allows the view to change data it will show in a stream-based way.
        /// This helps to ensure that other parts like JSON-Streams or Search have the same information
        /// as the view itself. 
        /// </summary>
        [PrivateApi]
        void CustomizeData();

        /// <summary>
        /// Mechanism which lets the search indexer ask the template how it should pre-process the content.
        /// </summary>
        /// <param name="searchInfos"></param>
        /// <param name="moduleInfo"></param>
        /// <param name="beginDate"></param>
        [PrivateApi]
        void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, IInstanceInfo moduleInfo, DateTime beginDate);

        /// <summary>
        /// todo
        /// </summary>
        [PrivateApi]
        RenderStatusType PreRenderStatus { get; }
    }
}