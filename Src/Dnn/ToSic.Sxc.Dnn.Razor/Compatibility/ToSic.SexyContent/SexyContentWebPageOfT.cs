// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Razor;

/// <summary>
/// this is just a generic variation of SexyContentWebPage
/// We need it for the Razor Engine - which in V5+ explicitly expects a generic type
/// We have no need for the internal type, so there is no code except for the inheritance
/// </summary>
/// <typeparam name="T"></typeparam>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class SexyContentWebPage<T>:SexyContentWebPage
{

}