using ToSic.Eav.LookUp;
using ToSic.Sxc.Services.Templates;

// TODO: unclear if the namespace is correct, feels a bit off...

namespace ToSic.Sxc.Services.Template;

/// <summary>
/// Engine which parses a template containing placeholders and replaces them with values from sources.
/// </summary>
/// <remarks>
/// Released in 18.03
/// </remarks>
[WorkInProgressApi("namespace not final")]
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
    /// <param name="template">The string containing the token placeholders like `Hello [User:FirstName]`</param>
    /// <returns></returns>
    string Parse(string template);

    /// <summary>
    /// Basic Parse functionality with more options.
    /// </summary>
    /// <param name="template">The string containing the token placeholders like `Hello [User:FirstName]`</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="allowHtml">allow adding html to the string - if false (default) will html encode anything found for safety before replacing something</param>
    /// <param name="sources">A list of sources to use for this parse - otherwise use the sources provided on creation of the template engine</param>
    /// <param name="recursions">
    /// Recursion depth for tokens-in-tokens.
    /// Could leak unexpected information, if a token contains user provided data or url-parameters so _use with caution_.
    /// Defaults to `0` for safety, added v20.09
    /// </param>
    // ReSharper disable once MethodOverloadWithOptionalParameter
    string Parse(string template, NoParamOrder protector = default, bool allowHtml = false, IEnumerable<ILookUp>? sources = default, int recursions = TemplateEngineTokens.MaxDepth);
}