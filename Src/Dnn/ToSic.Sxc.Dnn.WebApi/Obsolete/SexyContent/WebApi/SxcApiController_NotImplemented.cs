using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.WebApi
{
    public abstract partial class SxcApiController
    {

        #region new AsDynamic - not supported

        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson)
            => throw new Exception("The AsDynamic(string) is a new feature in 2sxc 10.20. To use it, change your base class. See https://r.2sxc.org/RazorComponent");

        ///// <inheritdoc/>
        //public IEnumerable<dynamic> AsDynamic(IDataSource source)
        //    => throw new Exception("The AsDynamic(string) is a new feature in 2sxc 10.20. To use it, change your base class. See https://r.2sxc.org/RazorComponent");


        #endregion

        #region AsList - only in newer APIs

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list)
            => throw new Exception("AsList is a new feature in 2sxc 10.20. To use it, change your template type to use the new Custom namespace.");

        #endregion

        
        public dynamic File(string dontRelyOnParameterOrder = Parameters.Protector, bool? download = null,
            string virtualPath = null, string contentType = null, string fileDownloadName = null, object contents = null) =>
            throw new NotSupportedException($"This method is not available in the old {nameof(SxcApiController)}. Use a newer base controller from the Custom namespace.");
    }
}
