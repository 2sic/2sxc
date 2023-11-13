using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using static ToSic.Eav.Parameters;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    [PrivateApi("Keep implementation hidden, only publish interface")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class DynamicStack: DynamicObject,
        /*IDynamicEntityBase,*/
        IWrapper<IPropertyStack>,
        IDynamicStack,
        IHasPropLookup,
        ISxcDynamicObject,
        IEnumerable, // note: not sure why it supports this, but it has been this way for a long time
        ICanDebug
    {
        #region Constructor and Helpers (Composition)

        public DynamicStack(string name, CodeDataFactory cdf, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources)
        {
            Cdf = cdf;
            var stack = new PropertyStack().Init(name, sources);
            _stack = stack;
            PropertyLookup = new PropLookupStack(stack, () => Debug);
        }
        // ReSharper disable once InconsistentNaming
        [PrivateApi] public CodeDataFactory Cdf { get; }
        private readonly IPropertyStack _stack;
        private const bool Strict = false;

        [PrivateApi]
        public IPropertyLookup PropertyLookup { get; }

        [PrivateApi]
        internal GetAndConvertHelper GetHelper => _getHelper ?? (_getHelper = new GetAndConvertHelper(this, Cdf, Strict, childrenShouldBeDynamic: true, canDebug: this));
        private GetAndConvertHelper _getHelper;

        [PrivateApi]
        internal SubDataFactory SubDataFactory => _subData ?? (_subData = new SubDataFactory(Cdf, Strict, canDebug: this));
        private SubDataFactory _subData;

        /// <inheritdoc />
        public bool Debug { get; set; }

        #endregion



        public override bool TryGetMember(GetMemberBinder binder, out object result) 
            => CodeDynHelper.TryGetMemberAndRespectStrict(GetHelper, binder, out result);


        /// <inheritdoc />
        [PrivateApi]
        public IPropertyStack GetContents() => _stack;

        /// <inheritdoc />
        [PrivateApi("was public till v16.02, but since I'm not sure if it is really used, we decided to hide it again since it's probably not an important API")]
        public dynamic GetSource(string name)
        {
            var source = _stack.GetSource(name)
                         // If not found, create a fake one
                         ?? Cdf.FakeEntity(Cdf.BlockOrNull?.AppId);

            return SourceToDynamicEntity(source);
        }

        /// <inheritdoc />
        [PrivateApi("Never published in docs")]
        public dynamic GetStack(params string[] names)
        {
            var wrapLog = GetHelper.LogOrNull.Fn<object>();
            var newStack = _stack.GetStack(GetHelper.LogOrNull, names);
            var newDynStack = new DynamicStack("New", Cdf, newStack.Sources);
            return wrapLog.Return(newDynStack);
        }

        private IDynamicEntity SourceToDynamicEntity(IPropertyLookup source)
        {
            if (source == null) return null;
            if (source is IDynamicEntity dynEnt) return dynEnt;
            if (source is ICanBeEntity canBe) return SubDataFactory.SubDynEntityOrNull(canBe.Entity);
            //if (source is IEntity ent) return SubDataFactory.SubDynEntityOrNull(ent);
            return null;
        }


        [PrivateApi("not implemented")]
        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotSupportedException($"Setting a value on {nameof(IDynamicStack)} is not supported");

        #region Get / Get<T>

        public dynamic Get(string name) => GetHelper.Get(name);

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public dynamic Get(string name, string noParamOrder = Protector, string language = null, bool convertLinks = true, bool? debug = null)
            => GetHelper.Get(name, noParamOrder, language, convertLinks, debug);

        public TValue Get<TValue>(string name)
            => GetHelper.Get<TValue>(name);

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public TValue Get<TValue>(string name, string noParamOrder = Protector, TValue fallback = default)
            => GetHelper.Get(name, noParamOrder, fallback);

        #endregion

        #region IEnumerable<IDynamicEntity>

        private List<IDynamicEntity> List => _list ?? (_list = _stack.Sources
            .Select(src => SourceToDynamicEntity(src.Value))
            .Where(e => e != null)
            .ToList());

        private List<IDynamicEntity> _list;

        public IEnumerator<IDynamicEntity> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion


        #region Any*** properties just for documentation

        public bool AnyBooleanProperty => true;
        public DateTime AnyDateTimeProperty => DateTime.Now;
        public IEnumerable<IDynamicEntity> AnyChildrenProperty => null;
        public string AnyJsonProperty => null;
        public string AnyLinkOrFileProperty => null;
        public decimal AnyNumberProperty => 0;
        public string AnyStringProperty => null;
        //public IEnumerable<DynamicEntity> AnyTitleOfAnEntityInTheList => null;

        #endregion

    }
}
