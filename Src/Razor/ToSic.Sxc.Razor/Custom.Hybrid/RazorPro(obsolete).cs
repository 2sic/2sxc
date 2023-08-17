// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract class RazorPro: RazorTyped
    {
        protected RazorPro() => throw CodePro.ExceptionObsolete(nameof(RazorPro), nameof(RazorTyped));
    }
}
