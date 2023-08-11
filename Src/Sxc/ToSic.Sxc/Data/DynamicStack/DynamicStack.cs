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
    public partial class DynamicStack: DynamicEntityBase, IWrapper<IPropertyStack>, IDynamicStack,
        IEnumerable // note: not sure why it supports this, but it has been this way for a long time
    {
        public DynamicStack(string name, CodeDataFactory cdf, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources) : base(cdf, strict: false)
        {
            var stack = new PropertyStack().Init(name, sources);
            UnwrappedStack = stack;
            PreWrap = new PreWrapStack(stack, this);
        }

        protected readonly IPropertyStack UnwrappedStack;
        private readonly PreWrapStack PreWrap;

        /// <inheritdoc />
        public IPropertyStack GetContents() => UnwrappedStack;

        /// <inheritdoc />
        public dynamic GetSource(string name)
        {
            var source = UnwrappedStack.GetSource(name)
                         // If not found, create a fake one
                         ?? _Cdf.FakeEntity(_Cdf.BlockOrNull?.AppId);

            return SourceToDynamicEntity(source);
        }

        /// <inheritdoc />
        public dynamic GetStack(params string[] names)
        {
            var wrapLog = Helper.LogOrNull.Fn<object>();
            var newStack = UnwrappedStack.GetStack(Helper.LogOrNull, names);
            var newDynStack = new DynamicStack("New", _Cdf, newStack.Sources);
            return wrapLog.Return(newDynStack);
        }

        private IDynamicEntity SourceToDynamicEntity(IPropertyLookup source)
        {
            if (source == null) return null;
            if (source is IDynamicEntity dynEnt) return dynEnt;
            if (source is IEntity ent) return Helper.SubDynEntityOrNull(ent);
            return null;
        }

        /// <inheritdoc />
        [PrivateApi("Internal")]
        public override PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            return PreWrap.FindPropertyInternal(specs, path);
            //specs = specs.SubLog("Sxc.DynStk", Debug);
            //path = path.KeepOrNew().Add("DynStack", specs.Field);

            //var l = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), "DynamicStack");
            //if (!specs.Field.HasValue())
            //    return l.Return(null, "no key");

            //var hasPath = specs.Field.Contains(".");
            //var r = hasPath
            //    ? UnwrappedStack.InternalGetPath(specs, path)
            //    : UnwrappedStack.FindPropertyInternal(specs, path);

            //return l.Return(r, $"{(r == null ? "null" : "ok")} using {(hasPath ? "Path" : "Property")}");
        }

        [PrivateApi("Internal")]
        public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path)
            => PreWrap._Dump(specs, path);

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => throw new NotSupportedException($"Setting a value on {nameof(IDynamicStack)} is not supported");

        #region IEnumerable<IDynamicEntity>

        private List<IDynamicEntity> List => _list ?? (_list = UnwrappedStack.Sources
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
        public IEnumerable<DynamicEntity> AnyChildrenProperty => null;
        public string AnyJsonProperty => null;
        public string AnyLinkOrFileProperty => null;
        public double AnyNumberProperty => 0;
        public string AnyStringProperty => null;
        public IEnumerable<DynamicEntity> AnyTitleOfAnEntityInTheList => null;

        #endregion

    }
}
