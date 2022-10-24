using System;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    public class ConvertService: IConvertService
    {
        public ConvertService(IJsonService json)
        {
            Json = json;
        }

        public bool OptimizeNumbers => true;

        public bool OptimizeBoolean => true;

        public T To<T>(object value) => value.ConvertOrDefault<T>(numeric: OptimizeNumbers, truthy: OptimizeBoolean);

        public T To<T>(object value,
            string paramsMustBeNamed = Eav.Parameters.Protector,
            T fallback = default)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(paramsMustBeNamed, nameof(To), $"{nameof(fallback)}");
            return value.ConvertOrFallback(fallback, numeric: OptimizeNumbers, truthy: OptimizeBoolean, fallbackOnDefault: false);
        }

        public int ToInt(object value) => To<int>(value);
        public int ToInt(object value, int fallback = 0) => To(value, fallback: fallback);
        public Guid ToGuid(object value) => To<Guid>(value);

        public Guid ToGuid(object value, Guid fallback = default)
        {
            return To<Guid>(value, fallback: fallback);
        }

        public float ToFloat(object value) => To<float>(value);
        public float ToFloat(object value, float fallback = 0F) => To(value, fallback: fallback);

        public decimal ToDecimal(object value) => To<decimal>(value);

        public decimal ToDecimal(object value, decimal fallback = 0m) => To<decimal>(value, fallback: fallback);

        public double ToDouble(object value) => To<double>(value);
        public double ToDouble(object value, double fallback = 0D) => To(value, fallback: fallback);

        public bool ToBool(object value) => To<bool>(value);
        public bool ToBool(object value, bool fallback = false) => To(value, fallback: fallback);
        
        public string ToString(object value) => To<string>(value);

        public string ToString(object value, string fallback = null, string paramsMustBeNamed = Eav.Parameters.Protector, bool fallbackOnNull = true)
        {
            var result = To(value, fallback: fallback);
            return result is null && fallbackOnNull ? fallback: result;
        }

        public string ForCode(object value) => ForCode(value, fallback: default);
        public string ForCode(object value, string fallback = null)
        {
            if (value == null) return null;

            // Pre-check special case of date-time which needs ISO encoding without time zone
            if (value.GetType().UnboxIfNullable() == typeof(DateTime))
            {
                var dt = ((DateTime)value).ToString("O").Substring(0, 23) + "z";
                return dt;
            }

            var result = To(value, fallback: fallback);
            if (result is null) return null;

            // If the original value was a boolean, we will do case changing as js expects "true" or "false" and not "True" or "False"
            if (value.GetType().UnboxIfNullable() == typeof(bool)) result = result.ToLowerInvariant();

            return result;
        }

        public IJsonService Json { get; }

        #region Invisible Converts for backward compatibility

        public int ToInt32(object value) => ToInt(value);

        public float ToSingle(object value) => ToFloat(value);

        #endregion
    }
}
