using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent
{
    public class DataPublishing
    {
        public bool Enabled { get; set; }
        public string Streams { get; set; }

        public DataPublishing()
        {
            Enabled = false;
            Streams = "";
        }

        //public string GetPublishedData()
        //{
        //    return "";
        //}
    }

    

}