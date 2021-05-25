using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;

// ReSharper disable once CheckNamespace
namespace Custom.Dnn
{
    /// <summary>
    /// This is the base class for all custom API Controllers. <br/>
    /// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    [DnnLogExceptions]
    public abstract class Api12 : Hybrid.Api12, IDnnDynamicWebApi
    {
        /// <inheritdoc />
        public new IDnnContext Dnn => base.Dnn;
    }
}
