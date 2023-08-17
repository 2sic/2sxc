using ToSic.Lib.Documentation;


// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <inheritdoc cref="Custom.Hybrid.CodePro"/>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public abstract class RazorPro: RazorTyped
    {
        protected RazorPro() => throw CodePro.ExceptionObsolete(nameof(RazorPro), nameof(RazorTyped));
    }
}
