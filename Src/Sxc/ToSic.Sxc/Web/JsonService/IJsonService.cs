using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Service to serialize/restore JSON. Get it using GetService &lt; T &gt;
    ///
    /// It works for 2sxc/EAV data but can be used for any data which can be serialized/deserialized.
    /// Since it's a data-operation, we keep it in this namespace, even if most other things in this namespace are 2sxc-data objects.
    ///
    /// *Important* This is simple object-string conversion.
    /// It doesn't change entity objects to be serializable.
    /// For that you should use the [](xref:ToSic.Eav.DataFormats.EavLight.IConvertToEavLight) which returns an object that can then be serialized. 
    /// </summary>
    /// <remarks>
    /// Introduced in 2sxc 12.05.
    /// For previous versions of 2sxc, you can just write code to access Newtonsoft directly.
    /// For more control regarding serialization, also just work with Newtonsoft directly.
    ///
    /// Internally it uses Newtonsoft and preserves the case of keys.
    /// In future the internal engine may change (like for .net core), but we'll ensure that the result remains consistent. 
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP 12.05 - not final, will probably move to ToSic.Sxc.Web")]
    public interface IJsonService
    {
        /// <summary>
        /// Convert an object to JSON if possible.
        /// </summary>
        string Serialize(object item);
        
        /// <summary>
        /// Convert a JSON to a typed object. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        T Deserialize<T>(string json);

        /// <summary>
        /// Convert a json to an anonymously typed object. This is usually not ideal.
        ///
        /// It's usually better to use [](xref:ToSic.Sxc.Code.IDynamicCode.AsDynamic(System.String,System.String)) or  <see cref="Deserialize{T}"/>
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        object Deserialize(string json);
    }
}
