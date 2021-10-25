using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn;

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


    }
}
