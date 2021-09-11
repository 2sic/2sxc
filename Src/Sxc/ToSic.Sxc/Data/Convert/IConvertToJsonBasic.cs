using ToSic.Eav.Convert;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.ImportExport.Json.Basic;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Convert Entities, Streams more to Dictionary&lt;string, object&gt; objects.
    /// Usually used before serializing to something else.
    ///
    /// Get this service with `var converter = GetService&lt;IConvertToDictionary&gt;()`
    /// </summary>
    /// <remarks>
    /// Introduced in 12.05
    /// </remarks>
    [PublicApi]
    public interface IConvertToJsonBasic: 
        Eav.Convert.IConvertToJsonBasic, 
        IConvertDataSource<JsonEntity>,
        IConvert<IDynamicEntity, JsonEntity>, // for the dynamic-entity
        IConvert<object, JsonEntity> // for the dynamic-entity
    {
        [PrivateApi("internal use only")]
        bool WithEdit { get; set; }

    }
}
