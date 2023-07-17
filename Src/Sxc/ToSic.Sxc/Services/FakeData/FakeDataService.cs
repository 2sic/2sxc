using ToSic.Eav.Data;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services.FakeData
{
    internal class FakeDataService: ServiceForDynamicCode
    {
        public FakeDataService() : base($"{Constants.SxcLogName}.FkData")
        {
        }

        public ITypedItem EmptyItem => _emptyItem.Get(() => _DynCodeRoot.AsC.AsItem(EmptyEntity, Eav.Parameters.Protector));
        private readonly GetOnce<ITypedItem> _emptyItem = new GetOnce<ITypedItem>();

        public IEntity EmptyEntity => _emptyEntity.Get(() => _DynCodeRoot.AsC.FakeEntity(_DynCodeRoot.App?.AppId));
        private readonly GetOnce<IEntity> _emptyEntity = new GetOnce<IEntity>();
    }
}
