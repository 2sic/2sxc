using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

public abstract class ApiPro: ApiTyped
{
    [PrivateApi]
    protected ApiPro() => throw CodePro.ExceptionObsolete(nameof(ApiPro), nameof(ApiTyped));

}