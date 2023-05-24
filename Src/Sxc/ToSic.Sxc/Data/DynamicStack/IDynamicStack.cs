using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a dynamic object which contains multiple dynamic objects (Sources).
    /// It will try to find a value inside each source in the order the Sources are managed. 
    /// </summary>
    /// <remarks>New in 12.02</remarks>
    [PublicApi]
    public partial interface IDynamicStack: ISxcDynamicObject, ICanDebug
    {
        /// <summary>
        /// Get a source object which is used in the stack. Returned as a dynamic object. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A dynamic object like a <see cref="IDynamicEntity"/> or similar. If not found, it will return a source which just-works, but doesn't have data. </returns>
        /// <remarks>
        /// Added in 2sxc 12.03
        /// </remarks>
        dynamic GetSource(string name);

        [PrivateApi]
        dynamic GetStack(params string[] names);



    }
}