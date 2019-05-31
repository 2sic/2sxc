using ToSic.SexyContent.Razor.Helpers;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn.Interfaces
{
    internal interface IHasDnnContext
    {
        DnnHelper Dnn { get; }
    }
}
