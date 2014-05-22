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

        public Dictionary<string, List<IEntity>> EntityLists { get; set; }

        public bool AutoExtractEntities { get; set; }
        public string AdditionalBody { get; set; }
        public string Url { get; set; }
    }
}