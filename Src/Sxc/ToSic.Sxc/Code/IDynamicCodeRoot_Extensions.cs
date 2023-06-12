using ToSic.Lib.Logging;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code
{
    // ReSharper disable once InconsistentNaming
    public static class IDynamicCodeRoot_Extensions
    {
        public static DynamicJacketBase AsDynamicFromJson(this IDynamicCodeRoot root, string json, string fallback = default, ILog log = default) => DynamicJacket.AsDynamicJacket(json, fallback, root?.Log);
    }
}
