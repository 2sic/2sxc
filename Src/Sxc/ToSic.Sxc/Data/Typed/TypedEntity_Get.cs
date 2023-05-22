namespace ToSic.Sxc.Data
{
    public partial class TypedEntity
    {
        public object Get(string name) => DynEntity.Get(name);

        TValue Get<TValue>(string name) => DynEntity.Get<TValue>(name);

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public TValue Get<TValue>(string name, string noParamOrder = Eav.Parameters.Protector, TValue fallback = default) 
            => DynEntity.Get(name, noParamOrder, fallback);

        public string String(string name, string fallback = default) => DynEntity.Get(name, fallback: fallback);

        public int Int(string name, int fallback = default) => DynEntity.Get(name, fallback: fallback);

        public bool Bool(string name, bool fallback = default) => DynEntity.Get(name, fallback: fallback);

        public long Long(string name, long fallback = default) => DynEntity.Get(name, fallback: fallback);

        public float Float(string name, float fallback = default) => DynEntity.Get(name, fallback: fallback);

        public decimal Decimal(string name, decimal fallback = default) => DynEntity.Get(name, fallback: fallback);

        public double Double(string name, double fallback = default) => DynEntity.Get(name, fallback: fallback);

        public string Link(string name) => DynEntity.Get(name, convertLinks: true) as string;
    }
}
