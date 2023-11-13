using ToSic.Eav;
using ToSic.Eav.Code.Help;
using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// > [!WARNING]
    /// > Do not use the `...Pro` base classes.
    /// It was released in 16.02 but deprecated in 16.03 and should not be used any more.
    /// We will remove them soon after a short transition period showing warnings.
    /// 
    /// Instead, please replace them with:
    ///
    /// * `RazorPro` should be `RazorTyped` see [](xref:Custom.Hybrid.RazorTyped)
    /// * `ApiPro` should be `ApiTyped` see [](xref:Custom.Hybrid.ApiTyped)
    /// * `CodePro` should be `CodeTyped` see [](xref:Custom.Hybrid.CodeTyped)
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class CodePro: CodeTyped
    {
        [PrivateApi]
        protected CodePro() => throw ExceptionObsolete(nameof(CodePro), nameof(CodeTyped));

        [PrivateApi]
        public static ExceptionWithHelp ExceptionObsolete(string deprecated, string replacement)
            => new ExceptionWithHelp(HelpObsolete(deprecated, replacement));

        [PrivateApi]
        private static CodeHelp HelpObsolete(string deprecated, string replacement) =>
            new CodeHelp(name: "", detect: null, linkCode: "https://r.2sxc.org/brc-1603",
                uiMessage: $"{deprecated} which was experimental in v16.02 is replaced with {replacement}. " +
                           $"Sorry for the inconvenience, but this is important for long term stable, consistent APIs. " +
                           $"Please update all uses of Custom.Hybrid.{deprecated} with Custom.Hybrid.{replacement}. See https://r.2sxc.org/brc-1603.");
    }
}
