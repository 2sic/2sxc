using System;
using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Search;

namespace ToSic.Sxc.Engines
{
    /// <summary>
    /// The sub-system in charge of taking
    /// - a configuration for an instance (aka Module)
    /// - a template
    /// and using all that to produce an html-string for the browser. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IEngine: IHasLog
    {
#if NETFRAMEWORK
#pragma warning disable CS0612
        /// <summary>
        /// Initialize the Engine (pass everything needed for Render to it).<br/>
        /// This is not in the constructor, because IEngines usually get constructed with DI,
        /// so the constructor is off-limits. 
        /// </summary>
        /// <param name="block">block within the cms</param>
        /// <param name="purpose">Purpose of the engine (show in web, search-index, etc.). The custom code may adapt its behavior depending on the purpose</param>
        void Init(IBlock block, Purpose purpose);
#pragma warning restore CS0612
#endif

        void Init(IBlock block);

        /// <summary>
        /// Renders a template, returning a string with the rendered template.
        /// </summary>
        /// <returns>The string - usually HTML - which the engine created. </returns>
        RenderEngineResult Render(object data);


#if NETFRAMEWORK
        /// <summary>
        /// Mechanism which allows the view to change data it will show in a stream-based way.
        /// This helps to ensure that other parts like JSON-Streams or Search have the same information
        /// as the view itself. 
        /// </summary>
        [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
        void CustomizeData();

        /// <summary>
        /// Mechanism which lets the search indexer ask the template how it should pre-process the content.
        /// </summary>
        /// <param name="searchInfos"></param>
        /// <param name="moduleInfo"></param>
        /// <param name="beginDate"></param>
        [PrivateApi]
        [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
        void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate);

        [PrivateApi]
        bool CompatibilityAutoLoadJQueryAndRvt { get; }
#endif
    }
}