using ToSic.Eav.PublicApi;

namespace ToSic.Sxc.Interfaces
{
    /// <summary>
    /// Compiler helpers to ensure that razor views and WebApis can access other code files
    /// in folders nearby. 
    /// </summary>
    [PublicApi]
    public interface ISharedCodeBuilder
    {
        /// <summary>
        /// Location of the current code. This is important when trying to create instances for
        /// other code in relative folders - as this is usually not known. 
        /// </summary>
        string SharedCodeVirtualRoot { get; set; }

        /// <summary>
        /// Create an instance of code lying in a file near this
        /// </summary>
        /// <param name="virtualPath">path to the other code file to compile</param>
        /// <param name="dontRelyOnParameterOrder">dummy parameter to ensure all other parameters are called using named parameters, so that the API can change in future</param>
        /// <param name="name">Override the class name to compile - usually not required as it should match the file name</param>
        /// <param name="relativePath">optional relative path, will usually use the <see cref="SharedCodeVirtualRoot"/></param>
        /// <param name="throwOnError">throw errors if compiling fails, recommended</param>
        /// <returns>An object of the class in the file</returns>
        dynamic CreateInstance(string virtualPath, 
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, 
            string name = null,
            string relativePath = null,
            bool throwOnError = true);

    }
}
