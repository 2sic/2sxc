using System;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi("WIP")]
    public class DynamicStack: DynamicEntityBase, IWrapper<IEntityStack>
    {
        public DynamicStack(DynamicEntityDependencies dependencies, params IEntity[] entities) : base(dependencies)
        {
            var stack = new EntityStack();
            stack.Init(dependencies.Dimensions, entities);
            UnwrappedContents = stack;
        }
        

        public IEntityStack UnwrappedContents { get; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var field = binder.Name;
            
            // check if we already have it in the cache - but only in normal lookups
            if (_ValueCache.ContainsKey(field))
            {
                result = _ValueCache[field];
                return true;
            }

            var foundSet = UnwrappedContents.ValueAndMore(field);
            result = foundSet.Item1;
            if (result != null) result = ValueAutoConverted(result, foundSet.Item2, true, foundSet.Item3, field);
            _ValueCache.Add(field, result);
            
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotImplementedException($"Setting a value on {nameof(DynamicStack)} is not supported");

    }
}
