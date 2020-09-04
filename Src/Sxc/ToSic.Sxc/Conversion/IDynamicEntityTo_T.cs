using System.Collections.Generic;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Conversion
{
    public interface IDynamicEntityTo<out T>
    {
        /// <summary>
        /// Return a converted list of dynamic objects
        /// </summary>
        IEnumerable<T> Convert(IEnumerable<object> dynamicList);

        /// <summary>
        /// Return a converted dynamic Entity
        /// </summary>
        T Convert(IDynamicEntity dynamicEntity);
    }
}
