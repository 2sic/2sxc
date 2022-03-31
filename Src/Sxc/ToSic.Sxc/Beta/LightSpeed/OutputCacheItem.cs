using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Beta.LightSpeed
{
    public class OutputCacheItem
    {
        public IRenderResult Data;

#if NETFRAMEWORK
        /// <summary>
        /// This is only used in Dnn - might be solved with generics some time, but ATM this is just simpler
        /// </summary>
        public bool EnforcePre1025 = true;
#endif
    }
}
