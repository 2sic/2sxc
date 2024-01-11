using ToSic.Lib.Coding;
using ToSic.Sxc.Data.Internal.Wrapper;
using ApiController = ToSic.Sxc.Dnn.ApiController;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.WebApi;

partial class SxcApiController
{

    #region new AsDynamic - not supported

    [PrivateApi]
    public dynamic AsDynamic(string json, string fallback = WrapperConstants.EmptyJson)
        => throw new($"The AsDynamic(string) is a new feature in 2sxc 10.20. {ApiController.ErrRecommendedNamespaces}");


    #endregion

    #region AsList - only in newer APIs

    [PrivateApi]
    public IEnumerable<dynamic> AsList(object list)
        => throw new($"AsList is a new feature in 2sxc 10.20. {ApiController.ErrRecommendedNamespaces}");

    #endregion

    [PrivateApi]
    public dynamic File(NoParamOrder noParamOrder = default, bool? download = null,
        string virtualPath = null, string contentType = null, string fileDownloadName = null, object contents = null) =>
        throw new NotSupportedException($"This method is not available in the old {nameof(SxcApiController)}. {ApiController.ErrRecommendedNamespaces}");


}