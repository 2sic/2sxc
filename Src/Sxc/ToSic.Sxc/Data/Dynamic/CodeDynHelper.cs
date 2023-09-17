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

        public IDynamicEntity Presentation => _prs.Get(() => SubDataFactory.SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Presentation));
        private readonly GetOnce<IDynamicEntity> _prs = new GetOnce<IDynamicEntity>();

        public IMetadata Metadata => _md.Get(() => SubDataFactory.Cdf.Metadata(Entity?.Metadata));
        private readonly GetOnce<IMetadata> _md = new GetOnce<IMetadata>();
        public IDynamicEntity Parent => _dp.Get(() => SubDataFactory.SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Parent));
        private readonly GetOnce<IDynamicEntity> _dp = new GetOnce<IDynamicEntity>();



        #region TryGetMember for dynamic access

        public static bool TryGetMemberAndRespectStrict(GetAndConvertHelper helper, GetMemberBinder binder, out object result)
        {
            var findResult = helper.GetInternal(binder.Name, lookupLink: true);
            // ReSharper disable once ExplicitCallerInfoArgument
            if (!findResult.Found && helper.PropsRequired)
                throw TypedHelpers.ErrStrict(binder.Name, cName: ".");
            result = findResult.Result;
            return true;
        }

        #endregion

    }
}
