namespace ToSic.Eav.Apps;

public interface IAppWithInternal : IApp
{
    IAppReader AppReader { get; }
}