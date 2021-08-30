using System.Collections.Generic;
using ToSic.Eav.Conversion;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Conversion
{
    // TODO: INCONSISTENCY BETWEEN INTERFACES - SOME IDictionary, some Dictionary
    public interface IDataToDictionary: 
        IEntitiesTo<IDictionary<string, object>>, 
        IStreamsTo<IDictionary<string,object>>,
        IDynamicEntityTo<IDictionary<string, object>>
    {
        [PrivateApi("internal use only")]
        bool WithEdit { get; set; }

    }
}
