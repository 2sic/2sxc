using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract class RazorPro: RazorTyped
    {
        [PrivateApi]
        protected RazorPro() => throw CodePro.ExceptionObsolete(nameof(RazorPro), nameof(RazorTyped));
    }
}
