using System;
using ToSic.Eav;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.WebApi
{
    public abstract partial class SxcApiController
    {
        public dynamic File(string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, bool? download = null,
            string virtualPath = null, string contentType = null, string fileDownloadName = null, object contents = null)
        {
            throw new NotImplementedException($"This method is not available in the old {nameof(SxcApiController)}. Use a newer base controller from the Custom namespace.");
        }
    }
}
