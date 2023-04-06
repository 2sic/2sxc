using Custom.Hybrid.Advanced;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// BETA Base class for v15 Dynamic WebAPI files.
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important
    /// 
    /// * The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// * WIP: Would use System.Text.Json to process JSON in HTTP, not Newtonsoft
    /// </remarks>
    [PrivateApi("still wip, for V15, but without Newtonsoft JSON")]
    [DnnLogExceptions]
    public abstract class Api15: Api14<dynamic, ServiceKit14>, IDynamicCode12, IDynamicWebApi
    {
        protected Api15() : base("Hyb15") { }
        protected Api15(string logSuffix) : base(logSuffix) { }

    }
}
