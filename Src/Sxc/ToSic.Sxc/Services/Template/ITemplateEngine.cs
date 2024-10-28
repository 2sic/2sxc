using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Services.Template;

/// <summary>
/// Engine which parses a template containing placeholders and replaces them with values from sources.
/// </summary>
/// <remarks>
/// Released in 18.03
/// </remarks>
[PublicApi]
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
    /// for passing into function calls, like into CMS HTML Tweaks.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    string Parse(string template);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    string Parse(string template, NoParamOrder protector = default, bool allowHtml = false, IEnumerable<ILookUp> sources = default);
}