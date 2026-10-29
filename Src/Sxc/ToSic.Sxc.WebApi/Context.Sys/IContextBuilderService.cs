namespace ToSic.Sxc.Context.Sys;

public interface IContextBuilderService
{
    IContextOfApp? TryFindAppFromPath(string? nameOrPath);

    IContextOfApp? TryFindAppFromUrlIds();
}