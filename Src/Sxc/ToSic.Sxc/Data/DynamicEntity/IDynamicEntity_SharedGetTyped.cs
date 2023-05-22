using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    public partial interface IDynamicEntity
    {
        /// <summary>
        /// Get a property and return the value as a `string`.
        /// If conversion fails, will return default `null` or what is specified in the `fallback`.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="fallback">_optional_ fallback if conversion fails</param>
        /// <returns>Value as `string`</returns>
        /// <remarks>Added in 16.01</remarks>
        [PrivateApi]
        string String(string name, string fallback = default);


        /// <summary>
        /// Get a property and return the value as a `int`.
        /// If conversion fails, will return default `0` or what is specified in the `fallback`.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="fallback">_optional_ fallback if conversion fails</param>
        /// <returns>Value as `int`</returns>
        /// <remarks>Added in 16.01</remarks>
        [PrivateApi]
        int Int(string name, int fallback = default);

        /// <summary>
        /// Get a property and return the value as a `bool`.
        /// If conversion fails, will return default `false` or what is specified in the `fallback`.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="fallback">_optional_ fallback if conversion fails</param>
        /// <returns>Value as `bool`</returns>
        /// <remarks>Added in 16.01</remarks>
        [PrivateApi]
        bool Bool(string name, bool fallback = default);


        /// <summary>
        /// Get a property and return the value as a `long`.
        /// If conversion fails, will return default `0` or what is specified in the `fallback`.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="fallback">_optional_ fallback if conversion fails</param>
        /// <returns>Value as `long`</returns>
        /// <remarks>Added in 16.01</remarks>
        [PrivateApi]
        long Long(string name, long fallback = default);

        /// <summary>
        /// Get a property and return the value as a `float`.
        /// If conversion fails, will return default `0` or what is specified in the `fallback`.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="fallback">_optional_ fallback if conversion fails</param>
        /// <returns>Value as `float`</returns>
        /// <remarks>Added in 16.01</remarks>
        [PrivateApi]
        float Float(string name, float fallback = default);


        /// <summary>
        /// Get a property and return the value as a `decimal`.
        /// If conversion fails, will return default `0` or what is specified in the `fallback`.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="fallback">_optional_ fallback if conversion fails</param>
        /// <returns>Value as `decimal`</returns>
        /// <remarks>Added in 16.01</remarks>
        [PrivateApi]
        decimal Decimal(string name, decimal fallback = default);

        /// <summary>
        /// Get a property and return the value as a `double`.
        /// If conversion fails, will return default `0` or what is specified in the `fallback`.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="fallback">_optional_ fallback if conversion fails</param>
        /// <returns>Value as `double`</returns>
        /// <remarks>Added in 16.01</remarks>
        [PrivateApi]
        double Double(string name, double fallback = default);
    }
}
