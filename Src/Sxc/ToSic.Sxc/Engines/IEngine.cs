using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Search;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    /// <summary>
    /// The sub-system in charge of taking
    /// - a configuration for an instance (aka Module)
    /// - a template
    /// and using all that to produce an html-string for the browser. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public interface IEngine
    {
        /// <summary>
        /// Initialize the Engine (pass everything needed for Render to it).<br/>
        /// This is not in the constructor, because IEngines usually get constructed with DI,
        /// so the constructor is off-limits. 
        /// </summary>
        /// <param name="block">block within the cms</param>
        /// <param name="purpose">Purpose of the engine (show in web, search-index, etc.). The custom code may adapt its behavior depending on the purpose</param>
        /// <param name="parentLog">Log to chain with</param>
        void Init(IBlock block, Purpose purpose, ILog parentLog);

        /// <summary>
        /// Renders a template, returning a string with the rendered template.
        /// </summary>
        /// <returns>The string - usually HTML - which the engine created. </returns>
        string Render();

        [PrivateApi] bool ActivateJsApi { get; }

        /// <summary>
        /// Mechanism which allows the view to change data it will show in a stream-based way.
        /// This helps to ensure that other parts like JSON-Streams or Search have the same information
        /// as the view itself. 
        /// </summary>
        void CustomizeData();

        /// <summary>
        /// Mechanism which lets the search indexer ask the template how it should pre-process the content.
        /// </summary>
        /// <param name="searchInfos"></param>
        /// <param name="moduleInfo"></param>
        /// <param name="beginDate"></param>
        [PrivateApi]
        void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate);

        /// <summary>
        /// todo
        /// </summary>
        [PrivateApi]
        RenderStatusType PreRenderStatus { get; }

        [PrivateApi]
        bool CompatibilityAutoLoadJQueryAndRVT { get; }

        List<IClientAsset> Assets { get; }
    }
}