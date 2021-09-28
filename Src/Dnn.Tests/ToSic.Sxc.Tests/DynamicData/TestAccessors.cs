using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DynamicData
{
    public class TestAccessors
    {
        public static DynamicReadObject DynReadObjT(object value, bool wrapChildren, bool wrapRealChildren)
            => new DynamicReadObject(value, wrapChildren, wrapRealChildren);
    }
}
