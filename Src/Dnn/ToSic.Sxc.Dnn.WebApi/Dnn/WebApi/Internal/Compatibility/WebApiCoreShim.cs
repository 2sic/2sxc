using ToSic.Lib.Services;

namespace ToSic.Sxc.Dnn.WebApi.Internal.Compatibility;

/// <summary>
/// This is the helper class to compose other WebApi-classes of modern custom API Controllers. <br/>
/// It provides Commands such as `Ok()` which already exist in .net core and allows hybrid controllers to have the same commands. 
///
/// Use `Custom.Hybrid.Api12` or `Custom.Hybrid.Api14` as your real base classes
/// </summary>
[PrivateApi("This is the .net core Shim class for v12+")]
[DnnLogExceptions]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class WebApiCoreShim: ServiceBase
{
    public HttpRequestMessage Request { get; }
    protected internal WebApiCoreShim(HttpRequestMessage request) : base("Dnn.ApShim")
    {
        Request = request;
    }

}