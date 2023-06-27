using System;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Data.AsConverter
{
    public partial class AsConverterService
    {
        public IEntity AsEntity(object thingToConvert) =>
            thingToConvert == null
                ? throw new ArgumentNullException(nameof(thingToConvert))
                : thingToConvert as IEntity
                  ?? (thingToConvert as ICanBeEntity)?.Entity
                  ?? throw new ArgumentException($"Tried to convert an object to {nameof(IEntity)} but cannot convert a {thingToConvert.GetType()}");

        public IEntity FakeEntity(int? appId) => _dataBuilderLazy.Value.FakeEntity(appId ?? 0);
    }
}
