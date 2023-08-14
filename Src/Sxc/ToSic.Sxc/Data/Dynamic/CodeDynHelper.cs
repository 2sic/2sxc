using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data.Decorators;
using ToSic.Sxc.Data.Typed;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    internal class CodeDynHelper
    {
        public IEntity Entity { get; }
        public SubDataFactory SubDataFactory { get; }

        public CodeDynHelper(IEntity entity, SubDataFactory subDataFactory)
        {
            Entity = entity;
            SubDataFactory = subDataFactory;
        }

        public IDynamicEntity Presentation => _p.Get(() => SubDataFactory.SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Presentation));
        private readonly GetOnce<IDynamicEntity> _p = new GetOnce<IDynamicEntity>();

        public IMetadata Metadata => _md.Get(() => SubDataFactory.Cdf.Metadata(Entity?.Metadata));
        private readonly GetOnce<IMetadata> _md = new GetOnce<IMetadata>();


        #region TryGetMember for dynamic access

        public static bool TryGetMemberAndRespectStrict(GetAndConvertHelper helper, GetMemberBinder binder, out object result)
        {
            var findResult = helper.GetInternal(binder.Name, lookupLink: true);
            // ReSharper disable once ExplicitCallerInfoArgument
            if (!findResult.Found && helper.StrictGet)
                throw TypedHelpers.ErrStrict(binder.Name, cName: ".");
            result = findResult.Result;
            return true;
        }

        #endregion

    }
}
