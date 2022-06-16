using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.WebApi
{
    /// <summary>
    /// This is the base class for other base-classes of modern custom API Controllers. <br/>
    /// It provides Commands such as `Ok()` which already exist in .net core and allows hybrid controllers to have the same commands. 
    ///
    /// **Warning**
    ///
    /// _Do not inherit from this class, it's included in the docs so you can see the APIs the other base classes have._
    ///
    /// Use `Custom.Hybrid.Api12` or `Custom.Hybrid.Api14` as your real base classes
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("This is the .net core Shim class for v12+")]
    [DnnLogExceptions]
    public abstract partial class ApiCoreShim: DynamicApiController, /*IDynamicCode12,*/ IDynamicWebApi, IHasDynamicCodeRoot
    {
        protected internal ApiCoreShim(string logSuffix) : base(logSuffix) { }

    }
}
