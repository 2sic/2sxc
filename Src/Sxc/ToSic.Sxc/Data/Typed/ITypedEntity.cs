using System;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi("WIP")]
    public interface ITypedEntity: ICanBeEntity
    {
        int Id { get; }
        Guid Guid { get; }
        dynamic Dyn { get; }
        TypedEntity Presentation { get; }
        IDynamicMetadata Metadata { get; }
        IDynamicField Field(string name);
        object Get(string name);
        TValue Get<TValue>(string name, string noParamOrder = Eav.Parameters.Protector, TValue fallback = default);
        string String(string name, string fallback = default);
        int Int(string name, int fallback = default);
        bool Bool(string name, bool fallback = default);
        long Long(string name, long fallback = default);
        decimal Decimal(string name, decimal fallback = default);
        double Double(string name, double fallback = default);
        string Link(string name);
    }
}