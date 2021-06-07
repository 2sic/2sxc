using System;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi("WIP")]
    public class DynamicStack: DynamicEntityBase, IWrapper<IEntityStack>, IDynamicEntityGet
    {
        public DynamicStack(DynamicEntityDependencies dependencies, params IEntity[] entities) : base(dependencies)
        {
            var stack = new EntityStack();
            stack.Init(entities);
            UnwrappedContents = stack;
        }
        

        public IEntityStack UnwrappedContents { get; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _getValue(binder.Name);
            return true;
        }

        //protected override object _getValue(string field, string language = null, bool lookup = true)
        //{
        //    // This determines if we should access & store in cache
        //    var defaultMode = language == null && lookup;

        //    // use the standard dimensions or overload
        //    var dimsToUse = language == null ? _Dependencies.Dimensions : new[] { language };

        //    // check if we already have it in the cache - but only in normal lookups
        //    if (defaultMode && _ValueCache.ContainsKey(field)) return _ValueCache[field];

        //    var foundSet = _getValueRaw(field, dimsToUse);
        //    var result = foundSet.Item1;
        //    if (result != null) result = ValueAutoConverted(result, foundSet.Item2, lookup, foundSet.Item3, field);
        //    _ValueCache.Add(field, result);
        //    return result;
        //}

        protected override Tuple<object, string, IEntity, string> _getValueRaw(string field, string[] dimensions)
        {
            var t = UnwrappedContents.ValueAndMore(field, dimensions);
            return new Tuple<object, string, IEntity, string>(t.Item1, t.Item2, t.Item3, "todo");
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotImplementedException($"Setting a value on {nameof(DynamicStack)} is not supported");

    }
}
