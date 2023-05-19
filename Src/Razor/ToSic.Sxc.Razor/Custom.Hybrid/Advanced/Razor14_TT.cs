using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid.Advanced
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract class Razor14<TModel, TServiceKit>: Razor12<TModel>, IRazor14<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        public TServiceKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TServiceKit>());
        private readonly GetOnce<TServiceKit> _kit = new();


        //[PrivateApi("WIP")]
        //public ICmsEntity AsCms(object target) => _DynCodeRoot.AsCms(target);
        [PrivateApi("WIP")]
        public ITypedEntity AsTyped(object target) => _DynCodeRoot.AsTyped(target);
        //[PrivateApi("WIP")]
        //public IEnumerable<ICmsEntity> AsCmsList(object list) => _DynCodeRoot.AsCmsList(list);
        [PrivateApi("WIP")]
        public IEnumerable<ITypedEntity> AsTypedList(object list) => _DynCodeRoot.AsTypedList(list);

    }
}
