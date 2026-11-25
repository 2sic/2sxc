using ToSic.Eav.Context.Sys;

namespace ToSic.Sxc.WebApi.Sys;

public static class IContextServiceExtensions
{
    public static string FindAppFolderFromScopeOrThrow(this IContextCompleteService ctxService, IWebApiAppContextBuilderService ctxBuilder)
        =>
            (ctxBuilder.TryFindAppFromUrlIds()
             ?? ctxService.BlockContextOrNull()
             ?? throw new("Can't find app folder from scope")
            )
            .AppReaderRequired
            .Specs
            .Folder;
}