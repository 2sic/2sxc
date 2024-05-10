using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Templates;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ITemplateEngine
{
    /// <summary>
    /// Get a list of underlying sources, mainly for debugging.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ILookUp> GetSources(NoParamOrder protector = default, int depth = 0);

    string Parse(string template, NoParamOrder protector = default, IEnumerable<ILookUp> sources = default);
}