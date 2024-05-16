using ToSic.Lib.DI;

namespace ToSic.Sxc.Polymorphism;

/// <summary>
/// A polymorphism resolver - which can determine alternate editions for a view / template
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPolymorphismResolver: ISwitchableService
{
    string Edition(string parameters, ILog log);
}