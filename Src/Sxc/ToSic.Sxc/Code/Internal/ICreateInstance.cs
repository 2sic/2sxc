using Custom.Hybrid;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// Marks objects - usually DynamicCode - which can create instances of other C# files. <br/>
/// A special feature is that it must store a reference to the path it's in (provided by the compiler that created this instance).
/// This is important, so that CreateInstance knows what path to start in. 
/// </summary>
[PrivateApi("Hidden now, as all implementing objects show the API. Before 16.02 2023-07 it was public!")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICreateInstance: IGetCodePath
{
    /// <summary>
    /// Create an instance of code lying in a file near this
    /// </summary>
    /// <param name="virtualPath">path to the other code file to compile</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="name">Override the class name to compile - usually not required as it should match the file name</param>
    /// <param name="relativePath">optional relative path, will usually use the <see cref="IGetCodePath.CreateInstancePath"/></param>
    /// <param name="throwOnError">throw errors if compiling fails, recommended</param>
    /// <returns>An object of the class in the file</returns>
    /// <remarks>
    /// Note that the C# code which we are creating inherits from a standard base class such as <see cref="Code12"/> or <see cref="DynamicCode"/>
    /// then it will automatically be initialized to support App, AsDynamic etc.
    /// </remarks>
    dynamic CreateInstance(string virtualPath,
        NoParamOrder noParamOrder = default,
        string name = null,
        string relativePath = null,
        bool throwOnError = true);

}