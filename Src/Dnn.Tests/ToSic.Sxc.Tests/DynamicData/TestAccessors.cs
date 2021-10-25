using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DynamicData
{
    public class TestAccessors
    {
        public static DynamicReadObject DynReadObjT(object value, bool wrapChildren, bool wrapRealChildren)
            => new DynamicReadObject(value, wrapChildren, wrapRealChildren);
    }
}
