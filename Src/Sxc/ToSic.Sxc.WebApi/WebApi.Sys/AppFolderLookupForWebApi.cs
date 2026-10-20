namespace ToSic.Sxc.WebApi.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppFolderLookupForWebApi(ISxcCurrentContextService ctxService) : ServiceBase("AppFld")
{
    public string FindAppFolderFromScopeOrThrow() =>
        ctxService
            .SetAppFromNameOrUrlParamsOrUseExistingBlockRequired("")
            .AppReaderRequired
            .Specs
            .Folder;
}