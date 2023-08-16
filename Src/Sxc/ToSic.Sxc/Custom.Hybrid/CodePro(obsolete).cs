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
    public abstract class CodePro: CodeTyped
    {
    }
}
