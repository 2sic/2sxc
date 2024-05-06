using ToSic.Eav.LookUp;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.LookUp;

namespace ToSic.Sxc.Services;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

    ILookUp CreateSource(string name, IDictionary<string, string> values);

    ILookUp CreateSource(string name, ILookUp original);

    ILookUp CreateSource(string name, IEntity entity, NoParamOrder protector = default, string[] dimensions = default);

    ILookUp CreateSource(string name, ITypedItem item, NoParamOrder protector = default, string[] dimensions = default);

    ILookUp CreateSource(string name, Func<string, string> getter);

    string Parse(string template, NoParamOrder protector = default, IEnumerable<ILookUp> sources = default);
}