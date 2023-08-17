using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json.Serialization;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data.Wrapper;

namespace ToSic.Sxc.Data
{
    // WIP
    // Inspired by https://stackoverflow.com/questions/46948289/how-do-you-convert-any-c-sharp-object-to-an-expandoobject
    // That was more complex with ability so set new values and switch between case-insensitive or not but that's not the purpose of this
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Will always return a value even if the property doesn't exist, in which case it resolves to null.
    /// </remarks>
    [JsonConverter(typeof(DynamicJsonConverter))]
    public class WrapObjectDynamic: DynamicObject, IWrapper<object>, IPropertyLookup, IHasJsonSource, ICanGetByName
    {
        [PrivateApi]
        public object GetContents() => PreWrap.GetContents();

        [PrivateApi]
        internal readonly Wrapper.PreWrapObject PreWrap;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="wrapChildren">
        /// Determines if properties which are objects should again be wrapped.
        /// When using this for DynamicModel it should be false, otherwise usually true.
        /// </param>
        [PrivateApi]
        internal WrapObjectDynamic(Wrapper.PreWrapObject preWrap, CodeDataWrapper wrapper)
        {
            Wrapper = wrapper;
            PreWrap = preWrap;
        }
        protected readonly CodeDataWrapper Wrapper;
        //protected readonly object UnwrappedObject;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = PreWrap.TryGetWrap(binder.Name, true).Result;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) 
            => throw new NotSupportedException($"Setting a value on {nameof(WrapObjectDynamic)} is not supported");


        /// <inheritdoc />
        [PrivateApi("Internal")]
        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path) 
            => PreWrap.FindPropertyInternal(specs, path);


        object IHasJsonSource.JsonSource => PreWrap.GetContents();

        public dynamic Get(string name) => PreWrap.TryGetWrap(name, true).Result;

        [PrivateApi]
        public List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path) => PreWrap._Dump(specs, path);

    }
}