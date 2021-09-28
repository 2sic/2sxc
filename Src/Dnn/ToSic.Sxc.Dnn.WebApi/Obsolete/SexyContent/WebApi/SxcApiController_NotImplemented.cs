using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Web;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.WebApi
{
    public abstract partial class SxcApiController
    {

        #region new AsDynamic - not supported

        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson)
            => throw new Exception($"The AsDynamic(string) is a new feature in 2sxc 10.20. {ApiController.ErrRecommendedNamespaces}");


        #endregion

        #region AsList - only in newer APIs

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list)
            => throw new Exception($"AsList is a new feature in 2sxc 10.20. {ApiController.ErrRecommendedNamespaces}");

        #endregion

        
        public dynamic File(string noParamOrder = Parameters.Protector, bool? download = null,
            string virtualPath = null, string contentType = null, string fileDownloadName = null, object contents = null) =>
            throw new NotSupportedException($"This method is not available in the old {nameof(SxcApiController)}. {ApiController.ErrRecommendedNamespaces}");

        #region Convert-Service - 2sxc 12.05 only

        [PrivateApi]
        public IConvertService Convert => throw new NotSupportedException(
            $"The command {nameof(Convert)} is only available in newer base classes. {ApiController.ErrRecommendedNamespaces}");

        #endregion

    }
}
