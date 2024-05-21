using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Templates;

/// <summary>
/// Engine which parses a template containing placeholders and replaces them with values from sources.
/// </summary>
/// <remarks>
/// New / beta in v17.08
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still Beta in v17.08")]
public interface ITemplateEngine
{
    /// <summary>
    /// Get a list of underlying sources, mainly for debugging.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ILookUp> GetSources(NoParamOrder protector = default, int depth = 0);

    /// <summary>
    /// Basic Parse functionality.
    /// This is the variant without parameters, which should be used in basic cases and also
    /// for passing into function calls, eg. into CMS HTML Tweaks.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    string Parse(string template);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    string Parse(string template, NoParamOrder protector = default, IEnumerable<ILookUp> sources = default);
}