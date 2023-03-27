using Custom.Hybrid.Advanced;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
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
    /// Important
    /// 
    /// * The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// * WIP: Would use System.Text.Json to process JSON in HTTP, not Newtonsoft
    /// </remarks>
    [WorkInProgressApi("still wip, for V15, but without Newtonsoft JSON")]
    [DnnLogExceptions]
    public abstract class Api15: Api14<dynamic, ServiceKit14>, IDynamicCode12, IDynamicWebApi
    {
        protected Api15() : base("Hyb12") { }
        protected Api15(string logSuffix) : base(logSuffix) { }

    }
}
