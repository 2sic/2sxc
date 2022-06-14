namespace ToSic.Sxc.Cms.Publishing
{
    public enum PagePublishingPriorities
    {
        Unknown = 0,
        Default = 1, 

        // Platform implementation, like Dnn
        Platform = 10,

        // Override forbidden
        DraftForbidden = 100,
    }
}
