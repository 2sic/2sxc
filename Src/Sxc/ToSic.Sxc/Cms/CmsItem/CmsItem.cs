using System;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Cms.CmsItem
{
    /// <summary>
    /// WIP
    /// This should become something of a replacement for DynamicEntity.
    /// But it's not really 'data', instead it's a smart object which
    /// 
    /// - can provide data
    /// - can do a lot of operations which are common in CMS scenarios
    /// 
    /// </summary>
    [PrivateApi("Still very WIP")]
    public class CmsItem
    {
        #region Constructor / internal

        private readonly IDynamicEntity _dynamicEntity;
        private readonly ServiceKit14 _kit;

        public CmsItem(IDynamicEntity dynamicEntity, ServiceKit14 kit)
        {
            _dynamicEntity = dynamicEntity;
            _kit = kit;
        }

        #endregion

        public int Id => _dynamicEntity.EntityId;
        public Guid Guid => _dynamicEntity.EntityGuid;

        public object Value(string name) => _dynamicEntity.Get(name);

        public TValue Value<TValue>(string name) => _dynamicEntity.Get<TValue>(name);

        public TValue Value<TValue>(string name, TValue fallback) => _dynamicEntity.Get(name, fallback: fallback);

        public IHtmlTag Show(
            string name,
            string noParamOrder = Eav.Parameters.Protector,
            object container = default,
            bool debug = default
        ) => _kit.Cms.Show(_dynamicEntity.Field(name), container: container, classes: null, debug: debug);
    }
}
