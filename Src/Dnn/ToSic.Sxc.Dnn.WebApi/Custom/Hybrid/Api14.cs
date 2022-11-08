using Custom.Hybrid.Advanced;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v14 Dynamic WebAPI files.
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// </remarks>
    [PublicApi]
    [DnnLogExceptions]
    [UseOldNewtonsoftForHttpJson]
    public abstract class Api14: Api14<dynamic, ServiceKit14>, IDynamicCode12, IDynamicWebApi, IHasDynamicCodeRoot
    {
        protected Api14() : base("Hyb12") { }
        protected Api14(string logSuffix) : base(logSuffix) { }

    }
}
