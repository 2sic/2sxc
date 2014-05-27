using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.DataSources
{
    public class ViewDataSource : PassThrough
    {
        public DataPublishing Publish = new DataPublishing();
    }
}