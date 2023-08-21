using ToSic.Eav.Data;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services
{
    internal partial class DataService
    {
        public IEntity EmptyEntity => _emptyEntity.Get(() => _DynCodeRoot.Cdf.FakeEntity(_DynCodeRoot.App?.AppId));
        private readonly GetOnce<IEntity> _emptyEntity = new GetOnce<IEntity>();

        public ITypedItem EmptyItem => _emptyItem.Get(() => _DynCodeRoot.Cdf.AsItem(EmptyEntity, Eav.Parameters.Protector, propsRequired: false));
        private readonly GetOnce<ITypedItem> _emptyItem = new GetOnce<ITypedItem>();


        public ITypedItem Item(object original)
        {
            return null;
        }
    }
}
