using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <inheritdoc cref="Custom.Hybrid.CodePro"/>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public abstract class ApiPro: ApiTyped
    {
        [PrivateApi]
        protected ApiPro() => throw CodePro.ExceptionObsolete(nameof(ApiPro), nameof(ApiTyped));
    }
}
