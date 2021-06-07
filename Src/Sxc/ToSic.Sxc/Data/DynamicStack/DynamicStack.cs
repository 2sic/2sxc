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
        public DynamicStack(IBlock block, IServiceProvider serviceProvider, string[] dimensions, params IEntity[] entities) : base(block, serviceProvider, dimensions)
        {
            var stack = new EntityStack();
            stack.Init(dimensions, entities);
            UnwrappedContents = stack;
        }
        

        public IEntityStack UnwrappedContents { get; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = UnwrappedContents.Value(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotImplementedException($"Setting a value on {nameof(DynamicStack)} is not supported");

    }
}
