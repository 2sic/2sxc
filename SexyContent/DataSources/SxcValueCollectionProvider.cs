using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.DataSources
{
    public class SxcValueCollectionProvider: ValueCollectionProvider
    {
        public SxcInstance SxcInstance;

        public SxcValueCollectionProvider(SxcInstance sxc)
        {
            SxcInstance = sxc;
        }
    }
}