using ToSic.Eav.LookUp;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Template;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services;

/// <summary>
/// Service to help parse token-based templates.
/// </summary>
/// <remarks>
/// Released in 18.03
/// </remarks>
[PublicApi]
public interface ITemplateService
{
    /// <summary>
    /// Start with the default engine, which already has lookups for QueryString and similar sources.
    /// </summary>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="sources">optional _additional_ sources</param>
    /// <returns></returns>
    ITemplateEngine Default(NoParamOrder protector = default, IEnumerable<ILookUp> sources = null);

    /// <summary>
    /// Start with an empty engine.
    /// This usually only makes sense, if you provide custom sources. 
    /// </summary>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="sources">optional sources, but without them this engine won't do much</param>
    /// <returns></returns>
    ITemplateEngine Empty(NoParamOrder protector = default, IEnumerable<ILookUp> sources = null);

    /// <summary>
    /// Get a built-in source by name.
    /// This is usually used when you want to create a template-engine with some specific sources,
    /// and you explicitly need for example the QueryString source as well.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>The source if found, otherwise null</returns>
    ILookUp GetSource(string name);

    /// <summary>
    /// Create a source based on a dictionary.
    /// Lookup will be case-insensitive.
    /// </summary>
    /// <param name="name">The source name, basically the first part of the token eg: [Name:Value]</param>
    /// <param name="values"></param>
    /// <returns></returns>
    ILookUp CreateSource(string name, IDictionary<string, string> values);

    /// <summary>
    /// Create a source based on another source.
    /// This is mainly used to give a source another name.
    /// </summary>
    /// <param name="name">The source name, basically the first part of the token eg: [Name:Value]</param>
    /// <param name="original"></param>
    /// <returns></returns>
    ILookUp CreateSource(string name, ILookUp original);

    /// <summary>
    /// Create a source using an entity (or entity-like thing such as an ITypedItem) as the source.
    /// </summary>
    /// <param name="name">The source name, basically the first part of the token eg: [Name:Value]</param>
    /// <param name="item">An <see cref="IEntity"/>, <see cref="ITypedItem"/> or similar object.</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="dimensions">optional array of languages to use when looking for the value - if the data is multi-language. Default to current languages.</param>
    /// <returns></returns>
    ILookUp CreateSource(string name, ICanBeEntity item, NoParamOrder protector = default, string[] dimensions = default);

    /// <summary>
    /// Create a source using a function, basically a very custom source. 
    /// </summary>
    /// <param name="name">The source name, basically the first part of the token eg: [Name:Value]</param>
    /// <param name="getter">The function which uses the key to retrieve a value. It will be case-sensitive/-insensitive based on your code.</param>
    /// <returns></returns>
    ILookUp CreateSource(string name, Func<string, string> getter);

    /// <summary>
    /// Create a source using a function, basically a very custom source.
    /// This variant has 2 string parameters - the key and the format-string.
    /// </summary>
    /// <param name="name">The source name, basically the first part of the token eg: [Name:Value]</param>
    /// <param name="getter"></param>
    /// <returns></returns>
    ILookUp CreateSource(string name, Func<string, string, string> getter);

    /// <summary>
    /// Quick parse a template using the default engine, and optional sources.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="protector"></param>
    /// <param name="sources"></param>
    /// <returns></returns>
    string Parse(string template, NoParamOrder protector = default, bool allowHtml = false, IEnumerable<ILookUp> sources = default);


    /// <summary>
    /// Merge multiple sources into one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="sources"></param>
    /// <returns></returns>
    /// <remarks>
    /// Added v17.09
    /// </remarks>
    ILookUp MergeSources(string name, IEnumerable<ILookUp> sources);
}