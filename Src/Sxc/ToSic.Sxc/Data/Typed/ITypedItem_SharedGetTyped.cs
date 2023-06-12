using System;

namespace ToSic.Sxc.Data
{
    public partial interface ITypedItem: ITypedObject
    {
        //// Important: Shared definitions
        //// These properties are shared: ITypedEntity, IDynamicEntity, IDynamicEntityBase, IDynamicStack
        //// Make sure they always stay in sync

        ///// <summary>
        ///// Get a property and return the value as a `bool`.
        ///// If conversion fails, will return default `false` or what is specified in the `fallback`.
        ///// </summary>
        ///// <param name="name">property name</param>
        ///// <param name="fallback">_optional_ fallback if conversion fails</param>
        ///// <returns>Value as `bool`</returns>
        ///// <remarks>Added in 16.01</remarks>
        //bool Bool(string name, bool fallback = default);


        ///// <summary>
        ///// Get a property and return the value as a `DateTime`.
        ///// If conversion fails, will return default `0001-01-01` or what is specified in the `fallback`.
        ///// </summary>
        ///// <param name="name">property name</param>
        ///// <param name="fallback">_optional_ fallback if conversion fails</param>
        ///// <returns>Value as `DateTime`</returns>
        ///// <remarks>Added in 16.01</remarks>
        //DateTime DateTime(string name, DateTime fallback = default);

        ///// <summary>
        ///// Get a property and return the value as a `string`.
        ///// If conversion fails, will return default `null` or what is specified in the `fallback`.
        ///// </summary>
        ///// <param name="name">property name</param>
        ///// <param name="fallback">_optional_ fallback if conversion fails</param>
        ///// <returns>Value as `string`</returns>
        ///// <remarks>Added in 16.01</remarks>
        //string String(string name, string fallback = default);

        //#region Numbers

        


        ///// <summary>
        ///// Get a property and return the value as a `int`.
        ///// If conversion fails, will return default `0` or what is specified in the `fallback`.
        ///// </summary>
        ///// <param name="name">property name</param>
        ///// <param name="fallback">_optional_ fallback if conversion fails</param>
        ///// <returns>Value as `int`</returns>
        ///// <remarks>Added in 16.01</remarks>
        //int Int(string name, int fallback = default);


        ///// <summary>
        ///// Get a property and return the value as a `long`.
        ///// If conversion fails, will return default `0` or what is specified in the `fallback`.
        ///// </summary>
        ///// <param name="name">property name</param>
        ///// <param name="fallback">_optional_ fallback if conversion fails</param>
        ///// <returns>Value as `long`</returns>
        ///// <remarks>Added in 16.01</remarks>
        //long Long(string name, long fallback = default);

        ///// <summary>
        ///// Get a property and return the value as a `float`.
        ///// If conversion fails, will return default `0` or what is specified in the `fallback`.
        ///// </summary>
        ///// <param name="name">property name</param>
        ///// <param name="fallback">_optional_ fallback if conversion fails</param>
        ///// <returns>Value as `float`</returns>
        ///// <remarks>Added in 16.01</remarks>
        //float Float(string name, float fallback = default);


        ///// <summary>
        ///// Get a property and return the value as a `decimal`.
        ///// If conversion fails, will return default `0` or what is specified in the `fallback`.
        ///// </summary>
        ///// <param name="name">property name</param>
        ///// <param name="fallback">_optional_ fallback if conversion fails</param>
        ///// <returns>Value as `decimal`</returns>
        ///// <remarks>Added in 16.01</remarks>
        //decimal Decimal(string name, decimal fallback = default);

        ///// <summary>
        ///// Get a property and return the value as a `double`.
        ///// If conversion fails, will return default `0` or what is specified in the `fallback`.
        ///// </summary>
        ///// <param name="name">property name</param>
        ///// <param name="fallback">_optional_ fallback if conversion fails</param>
        ///// <returns>Value as `double`</returns>
        ///// <remarks>Added in 16.01</remarks>
        //double Double(string name, double fallback = default);

        //#endregion

        ///// <summary>
        ///// Get a url from a field. This will auto-convert values such as `file:72` or `page:14`.
        ///// </summary>
        ///// <param name="name">The field name.</param>
        ///// <returns>A url converted if possible. If the field contains anything else such as `hello` then it will not be modified.</returns>
        //string Url(string name);

    }
}
