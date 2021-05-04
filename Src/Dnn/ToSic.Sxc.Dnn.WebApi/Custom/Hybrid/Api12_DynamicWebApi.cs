using System;
using ToSic.Eav;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Api12: IDynamicWebApi
    {
        /// <inheritdoc />
        public dynamic File(string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            bool? download = null,
            string virtualPath = null, 
            string contentType = null, 
            string fileDownloadName = null, 
            object contents = null)
        {
            // TODO: STV
            throw new NotImplementedException();
        }
    }
}
