// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract class ApiPro: ApiTyped
    {
        protected ApiPro() => throw CodePro.ExceptionObsolete(nameof(ApiPro), nameof(ApiTyped));

    }
}
