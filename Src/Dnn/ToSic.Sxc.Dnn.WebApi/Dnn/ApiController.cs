using Custom.Hybrid;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is the base class for all custom API Controllers. <br/>
    /// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    [DnnLogExceptions]
    public abstract class ApiController : Api12, IDnnDynamicWebApi
    {
        /// <remarks>
        /// Probably obsolete, but a bit risky to just remove
        /// We will only add it to ApiController but not to Api12, because no new code should ever use that.
        /// </remarks>
        [PrivateApi] public IBlock Block => GetBlock();
    }
}
