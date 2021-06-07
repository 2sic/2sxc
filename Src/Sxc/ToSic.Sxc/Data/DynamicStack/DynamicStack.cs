using System;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    [PrivateApi("WIP")]
    public class DynamicStack: DynamicEntityBase, IWrapper<IEntityStack>
    {
        public DynamicStack(DynamicEntityDependencies dependencies /*IBlock block, IServiceProvider serviceProvider, string[] dimensions*/, params IEntity[] entities) : base(dependencies)
        {
            var stack = new EntityStack();
            stack.Init(dependencies.Dimensions, entities);
            UnwrappedContents = stack;
        }
        

        public IEntityStack UnwrappedContents { get; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var foundSet = UnwrappedContents.ValueAndMore(binder.Name);
            result = foundSet.Item1;
            if (result != null) result = ValueAutoConverted(foundSet .Item1, foundSet.Item2, 
                true, foundSet.Item3, binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotImplementedException($"Setting a value on {nameof(DynamicStack)} is not supported");

    }
}
