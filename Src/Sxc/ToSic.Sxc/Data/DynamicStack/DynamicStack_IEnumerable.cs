using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Data
{
    public partial class DynamicStack : IEnumerable<IDynamicEntity>
    {
        private List<IDynamicEntity> List => _list ?? (_list = UnwrappedContents.Sources
            .Select(src => SourceToDynamicEntity(src.Value))
            .Where(e => e != null)
            .ToList());
    
        private List<IDynamicEntity> _list;
        
        public IEnumerator<IDynamicEntity> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
