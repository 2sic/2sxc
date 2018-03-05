using DotNetNuke.Services.Search.Entities;

namespace ToSic.SexyContent.Search
{
    public class SearchInfo : SearchDocument, ISearchInfo
    {
        public ToSic.Eav.Interfaces.IEntity Entity { get; set; }
    }
}