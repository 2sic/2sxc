using System;

namespace ToSic.SexyContent.Search
{
    public interface ISearchInfo
    {
        string UniqueKey { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Body { get; set; }
        string Url { get; set; }
        DateTime ModifiedTimeUtc { get; set; }
        bool IsActive { get; set; }
        string QueryString { get; set; }
        string CultureCode { get; set; }
        ToSic.Eav.Interfaces.IEntity Entity { get; set; }
    }
}
