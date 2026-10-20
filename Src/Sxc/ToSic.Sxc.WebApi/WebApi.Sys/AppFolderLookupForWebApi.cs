namespace ToSic.Sxc.WebApi.Sys;

public static class ISxcCurrentContextServiceExtensions
{
    public static string FindAppFolderFromScopeOrThrow(this ISxcCurrentContextService ctxService) =>
        ctxService
            .FindAppFromPathNameUrlIdsOrSharedBlock("")
            .AppReaderRequired
            .Specs
            .Folder;
}