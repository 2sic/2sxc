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


        /// <inheritdoc />
        public ITypedEntity AsTyped(object target, string noParamOrder = ToSic.Eav.Parameters.Protector) => _DynCodeRoot.AsTyped(target);

        /// <inheritdoc />
        public IEnumerable<ITypedEntity> AsTypedList(object list, string noParamOrder = ToSic.Eav.Parameters.Protector) => _DynCodeRoot.AsTypedList(list);

    }
}
