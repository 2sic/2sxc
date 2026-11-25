

namespace ToSic.Sxc.Code.Context.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICodeContextFactory
{
    /// <summary>
    /// Creates a CodeApiService - if possible based on the parent class requesting it.
    /// </summary>
    /// <returns></returns>
    ICodeContext New(CodeContextOptions options);
}
