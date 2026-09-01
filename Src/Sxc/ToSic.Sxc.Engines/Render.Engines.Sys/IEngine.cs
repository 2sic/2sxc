using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Render.Output.Sys;
using ToSic.Sxc.Render.Sys.Specs;

namespace ToSic.Sxc.Render.Engines.Sys;

/// <summary>
/// The sub-system in charge of taking
/// - a configuration for an instance (aka Module)
/// - a template
/// and using all that to produce an html-string for the browser. 
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IEngine: IHasLog
{
    //void Init(IBlock block);

    /// <summary>
    /// Renders a template, returning a string with the rendered template.
    /// </summary>
    /// <returns>The string - usually HTML - which the engine created. </returns>
    OutputFragmentWithAssets Render(IBlock block, RenderSpecs specs);
}