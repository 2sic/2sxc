using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Context.Sys;

internal record ContextOfBlock(
    ISite Site,
    IUser User,
    IPage Page,
    IModule Module,
    IAppReader? AppReaderOrNull,
    IPageServiceShared PageServiceShared
) : ContextOfApp(Site, User, AppReaderOrNull), IContextOfBlock
{
    internal required LazySvc<BlockPublishingSettingsService> PagePubSettings { get; init; }

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public BlockPublishingSettings Publishing => field
        ??= PagePubSettings.Value
            .Handle(new(), new(new() { ModuleId = Module?.Id ?? -1 }))
            .GetAwaiter()
            .GetResult()
            .Data;


    IContextOfSite IContextOfSite.WithSwappedSite(ISite site) => this with { Site = site };
}