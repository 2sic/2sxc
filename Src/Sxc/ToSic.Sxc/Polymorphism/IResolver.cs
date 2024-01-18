namespace ToSic.Sxc.Polymorphism;

/// <summary>
/// A polymorphism resolver - which can determine alternate editions for a view / template
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IResolver
{
    string Name { get; }

    string Edition(string parameters, ILog log);
}