namespace ToSic.Sxc.Apps.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IAppWithInternal : IApp
{
    IAppReader AppReader { get; }
}