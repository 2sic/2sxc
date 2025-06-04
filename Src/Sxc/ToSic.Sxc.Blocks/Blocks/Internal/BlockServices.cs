using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.DataSources.Internal;

namespace ToSic.Sxc.Blocks.Internal;

public class BlockServices(GenWorkPlus<WorkViews> workViews, GenWorkPlus<WorkBlocks> appBlocks, LazySvc<BlockDataSourceFactory> bdsFactoryLazy, LazySvc<App> appLazy)
    : MyServicesBase(connect: [bdsFactoryLazy, appLazy, workViews, appBlocks])
{
    internal LazySvc<BlockDataSourceFactory> BdsFactoryLazy { get; } = bdsFactoryLazy;
    internal LazySvc<App> AppLazy { get; } = appLazy;
    public GenWorkPlus<WorkViews> WorkViews { get; } = workViews;
    public GenWorkPlus<WorkBlocks> AppBlocks { get; } = appBlocks;
}