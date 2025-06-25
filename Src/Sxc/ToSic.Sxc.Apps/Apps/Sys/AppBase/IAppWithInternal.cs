namespace ToSic.Sxc.Apps.Sys;

public interface IAppWithInternal : IApp
{
    IAppReader AppReader { get; }
}