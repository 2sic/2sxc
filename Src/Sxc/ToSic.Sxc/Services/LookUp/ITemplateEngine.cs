using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Services.LookUp;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ITemplateEngine
{

    public IEnumerable<ILookUp> Sources { get; }

    string Parse(string template, NoParamOrder protector = default, IEnumerable<ILookUp> sources = default);
}