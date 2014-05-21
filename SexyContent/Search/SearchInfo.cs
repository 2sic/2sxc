using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent.Search
{
    public class SearchInfo
    {
        public SearchInfo()
        {
            AutoExtractEntities = true;
        }

        //public List<IEntity> Entities { get; set; }

        public Dictionary<string, List<IEntity>> EntityLists { get; set; }

        public bool AutoExtractEntities { get; set; }
        // ToDo: Naming...
        public string AdditionalSearchText { get; set; }
        public string Url { get; set; }
    }
}