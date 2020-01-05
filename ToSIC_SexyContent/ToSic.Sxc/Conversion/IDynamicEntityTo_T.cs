using System.Collections.Generic;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Conversion
{
    public interface IDynamicEntityTo<out T>
    {
        /// <summary>
        /// Return a converted list of dynamic objects
        /// </summary>
        IEnumerable<T> Prepare(IEnumerable<object> dynamicList);

        /// <summary>
        /// Return a converted dynamic Entity
        /// </summary>
        T Prepare(IDynamicEntity dynamicEntity);
    }
}
