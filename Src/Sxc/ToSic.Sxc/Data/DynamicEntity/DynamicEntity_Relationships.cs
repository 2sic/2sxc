using System.Collections.Generic;
using System.Linq;
using ToSic.Sxc.Data.Decorators;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public List<IDynamicEntity> Parents(string type = null, string field = null)
            => Entity.Parents(type, field).Select(e => Helper.SubDynEntityOrNull(e)).ToList();


        /// <inheritdoc />
        public List<IDynamicEntity> Children(string field = null, string type = null)
            => Entity.Children(field, type)
                .Select((e, i) => EntityInBlockDecorator.Wrap(e, Entity.EntityGuid, field, i))
                .Select(e => Helper.SubDynEntityOrNull(e))
                .ToList();

    }
}
