using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.Search.Entities;
using ToSic.Eav;

namespace ToSic.SexyContent.Search
{
    public class SearchInfo : SearchDocument
    {
        public SearchInfo(IEntity entity)
        {
            //AutoExtractEntities = true;
        }

        //public Dictionary<string, List<IEntity>> EntityLists { get; set; }
        //public List<IEntity> Entities { get; set; }




        public IEntity Entity { get; set; }

        //public bool AutoExtractEntities { get; set; }
        //public string Body { get; set; }
        //public string Url { get; set; }

        //// ToDo: Review with 2dm
        //public string Title { get; set; }
        //public DateTime Modified { get; set; }
    }
}