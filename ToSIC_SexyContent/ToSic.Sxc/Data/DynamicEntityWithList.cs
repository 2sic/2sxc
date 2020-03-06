using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a List of <see cref="IDynamicEntity"/>, which also behaves as an IDynamicEntity itself. <br/>
    /// So if it has any elements you can enumerate it (foreach). <br/>
    /// You can also do things like `.EntityId` or `.SomeProperty` just like a DynamicEntity.
    /// </summary>
    /// <remarks>Added in 2sxc 10.27</remarks>
    [PublicApi_Stable_ForUseInYourCode]
    public class DynamicEntityWithList: DynamicEntity, IReadOnlyList<IDynamicEntity>
    {
        [PrivateApi]
        protected List<DynamicEntity> DynEntities;

        [PrivateApi]
        public DynamicEntityWithList(IEnumerable<IEntity> entities, string[] dimensions, int compatibility, IBlockBuilder blockBuilder) 
            : base(null, dimensions, compatibility, blockBuilder)
        {
            DynEntities = entities.Select(
                p => new DynamicEntity(p, Dimensions, CompatibilityLevel, BlockBuilder)
            ).ToList();
            Entity = DynEntities.FirstOrDefault()?.Entity;
        }

        public IEnumerator<IDynamicEntity> GetEnumerator() => DynEntities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => DynEntities.Count;

        public IDynamicEntity this[int index] => DynEntities[index];
    }
}
