using ToSic.Lib.Logging;

namespace ToSic.Sxc.Polymorphism
{
    /// <summary>
    /// A polymorphism resolver - which can determine alternate editions for a view / template
    /// </summary>
    public interface IResolver
    {
        string Name { get; }


        string Edition(string parameters, ILog log);
    }
}
