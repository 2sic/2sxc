// ReSharper disable once CheckNamespace

using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn.Interfaces
{
    /// <summary>
    /// This interface marks things that also provide a DNN Context.
    /// It's important, because if 2sxc also runs on other CMS platforms, then the Dnn Context won't be available, so it's in a separate interface.
    /// </summary>
    [PublicApi]
    internal interface IHasDnnContext
    {
        /// <summary>
        /// The DNN context. This 
        /// </summary>
        IDnnContext Dnn { get; }
    }
}
