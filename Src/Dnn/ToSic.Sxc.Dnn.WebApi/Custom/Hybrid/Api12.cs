using ToSic.Eav.Documentation;
using ToSic.Eav.WebApi.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// This is the base class for all custom API Controllers. <br/>
    /// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
    /// </summary>
    [PublicApi("This is the official base class for v12+")]
    [DnnLogExceptions]
    [NewtonsoftJsonResponse]
    public abstract partial class Api12: ApiCoreShim, IDynamicCode12, IDynamicWebApi, IHasDynamicCodeRoot
    {
        protected Api12() : base("Hyb12") { }
        protected Api12(string logSuffix) : base(logSuffix) { }

    }
}
