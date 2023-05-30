using System;

namespace ToSic.Sxc.Data
{
    public partial class TypedItem
    {
        public object Get(string name) => DynEntity.Get(name);

        TValue Get<TValue>(string name) => DynEntity.Get<TValue>(name);

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public TValue Get<TValue>(string name, string noParamOrder = Eav.Parameters.Protector, TValue fallback = default) 
            => DynEntity.Get(name, noParamOrder, fallback);

        /// <inheritdoc />
        public DateTime DateTime(string name, DateTime fallback = default) => DynEntity.Get(name, fallback: fallback);

        /// <inheritdoc />
        public string String(string name, string fallback = default) => DynEntity.Get(name, fallback: fallback);

        /// <inheritdoc />
        public int Int(string name, int fallback = default) => DynEntity.Get(name, fallback: fallback);

        /// <inheritdoc />
        public bool Bool(string name, bool fallback = default) => DynEntity.Get(name, fallback: fallback);

        /// <inheritdoc />
        public long Long(string name, long fallback = default) => DynEntity.Get(name, fallback: fallback);

        /// <inheritdoc />
        public float Float(string name, float fallback = default) => DynEntity.Get(name, fallback: fallback);

        /// <inheritdoc />
        public decimal Decimal(string name, decimal fallback = default) => DynEntity.Get(name, fallback: fallback);

        /// <inheritdoc />
        public double Double(string name, double fallback = default) => DynEntity.Get(name, fallback: fallback);

        /// <inheritdoc />
        public string Url(string name) => DynEntity.Get(name, convertLinks: true) as string;
    }
}
