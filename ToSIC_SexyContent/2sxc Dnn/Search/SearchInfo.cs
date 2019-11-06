using DotNetNuke.Services.Search.Entities;
using ToSic.Eav.Data;

namespace ToSic.SexyContent.Search
{
    public class SearchInfo : SearchDocument, ISearchInfo
    {
        public IEntity Entity { get; set; }
    }
}