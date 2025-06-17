namespace ToSic.Sxc.Apps.Sys;

public interface IAppWithInternal : Eav.Apps.IApp
{
    IAppReader AppReader { get; }
}