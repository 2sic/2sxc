using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Sys.Module;


public record Module<T>(
    int Id,
    bool IsContent,
    IBlockIdentifier? BlockIdentifier
) : IModule, IWrapper<T>
    where T : class
{
    public T? GetContents() => Contents;

    public required T? Contents { get; init; }
}