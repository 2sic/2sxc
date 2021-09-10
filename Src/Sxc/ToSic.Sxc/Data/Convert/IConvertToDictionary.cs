using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.ImportExport.Convert;

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
    public interface IConvertToDictionary: 
        IConvertJsonV0, 
        IConvertStreams<IDictionary<string,object>>,
        IConvert<IDynamicEntity, IDictionary<string, object>>, // for the dynamic-entity
        IConvert<object, IDictionary<string, object>> // for the dynamic-entity
    {
        [PrivateApi("internal use only")]
        bool WithEdit { get; set; }

    }
}
