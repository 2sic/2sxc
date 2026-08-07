using System.Reflection;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Raw.Sys;

namespace ToSic.Sxc.Backend.SysData;

internal static class SysDataRaw
{
    internal static IRawEntity One(object value)
        => new RawEntity
        {
            Values = value.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => (object?)p.GetValue(value)),
        };

    internal static IEnumerable<IRawEntity> Many(IEnumerable<object>? values)
        => values?.Select(One) ?? [];
}
