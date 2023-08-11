using System;
using ToSic.Eav.Data;
using ToSic.Sxc.Data.Decorators;

namespace ToSic.Sxc.Data
{
    public partial class CodeDataFactory
    {
        public IEntity AsEntity(object thingToConvert) =>
            thingToConvert == null
                ? throw new ArgumentNullException(nameof(thingToConvert))
                : thingToConvert as IEntity
                  ?? (thingToConvert as ICanBeEntity)?.Entity
                  ?? throw new ArgumentException($"Tried to convert an object to {nameof(IEntity)} but cannot convert a {thingToConvert.GetType()}");

        public IEntity FakeEntity(int? appId) => _dataBuilderLazy.Value.FakeEntity(appId ?? 0);

        public IEntity PlaceHolderInBlock(int? appIdOrNull, IEntity parent, string field)
        {
            var dummyEntity = FakeEntity(appIdOrNull ?? parent.AppId);
            return parent == null ? dummyEntity : EntityInBlockDecorator.Wrap(dummyEntity, parent.EntityGuid, field);
        }

    }
}
