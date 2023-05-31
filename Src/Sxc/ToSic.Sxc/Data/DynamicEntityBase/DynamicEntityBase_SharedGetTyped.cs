using System;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicEntityBase
    {
        [PrivateApi]
        public DateTime DateTime(string name, DateTime fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        public string String(string name, string fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        public int Int(string name, int fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        public bool Bool(string name, bool fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        public long Long(string name, long fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        public float Float(string name, float fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        public decimal Decimal(string name, decimal fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        public double Double(string name, double fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        public string Url(string name)
        {
            var url = Get(name, convertLinks: true) as string;
            return Tags.SafeUrl(url).ToString();
        }
    }
}
