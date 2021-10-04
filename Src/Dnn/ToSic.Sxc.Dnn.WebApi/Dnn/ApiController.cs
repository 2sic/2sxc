using System;
using Custom.Hybrid;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is the base class for all custom API Controllers. <br/>
    /// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    [DnnLogExceptions]
    [Obsolete("This will continue to work, but you should use the Custom.Hybrid.Api12 or Custom.Dnn.Api12 instead.")]
    public abstract class ApiController : Api12, IDnnDynamicWebApi
    {

        public const string ErrRecommendedNamespaces = "To use it, use the new base class from Custom.Hybrid.Api12 or Custom.Dnn.Api12 instead.";

        /// <remarks>
        /// Probably obsolete, but a bit risky to just remove
        /// We will only add it to ApiController but not to Api12, because no new code should ever use that.
        /// </remarks>
        [PrivateApi] public IBlock Block => GetBlock();

        #region Convert-Service - 2sxc 12.05 only

        // todo: 12.05
        //[PrivateApi]
        //public new IxConvertService Convert => throw new NotSupportedException(
        //    $"The command {nameof(Convert)} is only available in newer base classes. {ErrRecommendedNamespaces}");

        #endregion

    }
}
