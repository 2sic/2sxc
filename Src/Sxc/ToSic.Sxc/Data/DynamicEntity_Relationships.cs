using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {

        /// <inheritdoc />
        public List<IDynamicEntity> Parents(string type = null, string field = null)
            => Entity.Parents(type, field)
                .Select(SubDynEntity)
                //.Select(e => new DynamicEntity(e, Dimensions, CompatibilityLevel, Block))
                //.Cast<IDynamicEntity>()
                .ToList();

        /// <inheritdoc />
        public List<IDynamicEntity> Children(string field = null, string type = null)
            => Entity.Children(field, type)
                .Select(SubDynEntity)
                //.Select(e => new DynamicEntity(e, Dimensions, CompatibilityLevel, Block))
                //.Cast<IDynamicEntity>()
                .ToList();


        [PrivateApi]
        protected IDynamicEntity SubDynEntity(IEntity contents)
        {
            if (contents == null) return null;
            var child = new DynamicEntity(contents, Dimensions, CompatibilityLevel, Block);
            // special case: if it's a Dynamic Entity without block (like App.Settings)
            // it needs the Service Provider from this object to work
            if (Block == null && ServiceProviderOrNull != null) child.ServiceProviderOrNull = ServiceProviderOrNull;
            return child;
        }

    }
}
