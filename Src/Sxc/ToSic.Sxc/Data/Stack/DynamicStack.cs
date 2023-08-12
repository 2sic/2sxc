using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    [PrivateApi("Keep implementation hidden, only publish interface")]
    public class DynamicStack: DynamicEntityBase, IWrapper<IPropertyStack>, IDynamicStack,
        IEnumerable // note: not sure why it supports this, but it has been this way for a long time
    {
        public DynamicStack(string name, CodeDataFactory cdf, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources) : base(cdf, strict: false)
        {
            var stack = new PropertyStack().Init(name, sources);
            _stack = stack;
            _propLookup = new PropLookupStack(stack, () => Debug);
            //CompleteSetup();
        }

        private readonly IPropertyStack _stack;
        private readonly PropLookupStack _propLookup;



        public override bool TryGetMember(GetMemberBinder binder, out object result) 
            => CodeDynHelper.TryGetMemberAndRespectStrict(GetHelper, binder, out result);

        public override IPropertyLookup PropertyLookup => _propLookup;

        /// <inheritdoc />
        [PrivateApi]
        public IPropertyStack GetContents() => _stack;

        /// <inheritdoc />
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
            if (source is IEntity ent) return SubDataFactory.SubDynEntityOrNull(ent);
            return null;
        }


        /// <inheritdoc />
        [PrivateApi("Internal")]
        public override PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path) 
            => _propLookup.FindPropertyInternal(specs, path);

        [PrivateApi("Internal")]
        public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path)
            => _propLookup._Dump(specs, path);

        [PrivateApi("not implemented")]
        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotSupportedException($"Setting a value on {nameof(IDynamicStack)} is not supported");

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
