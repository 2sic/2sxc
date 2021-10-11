namespace ToSic.Sxc.Web
{
    public interface IEventLogService
    {
        void AddEvent(string title, string message);
    }
}
