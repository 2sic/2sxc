using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public List<IDynamicEntity> Parents(string type = null, string field = null)
            => Entity.Parents(type, field).Select(SubDynEntity).ToList();


        /// <inheritdoc />
        public List<IDynamicEntity> Children(string field = null, string type = null)
            => Entity.Children(field, type).Select(SubDynEntity).ToList();


        //[PrivateApi]
        //protected IDynamicEntity SubDynEntity(IEntity contents) =>
        //    contents == null 
        //        ? null
        //        // Note: if it's a Dynamic Entity without block (like App.Settings) it needs the Service Provider
        //        : new DynamicEntity(contents, _Dependencies);
    }
}
