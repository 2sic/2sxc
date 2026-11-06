namespace ToSic.Sxc.Apps.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IAppForCode: IAppIdentity
{
    IEntity? SettingsEntity { get; }

    IEntity? ResourcesEntity { get; }

    IAppPaths AppPaths { get; }

    IAppSpecs AppSpecs { get; }

    IAppReader AppReader { get; }

    SxcAppDataService DataService { get; }
}