using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json.Serialization;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;

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
    public partial class DynamicReadObject: DynamicObject, IWrapper<object>, IPropertyLookup, IHasJsonSource, ICanGetByName
    {
        [PrivateApi]
        public object GetContents() => Analyzer.GetContents();// UnwrappedObject;

        [PrivateApi]
        internal readonly AnalyzeObject Analyzer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="wrapChildren">
        /// Determines if properties which are objects should again be wrapped.
        /// When using this for DynamicModel it should be false, otherwise usually true.
        /// </param>
        [PrivateApi]
        internal DynamicReadObject(object item, bool wrapChildren, bool wrapRealChildren, DynamicWrapperFactory wrapperFactory)
        {
            WrapperFactory = wrapperFactory;
            //UnwrappedObject = item;
            Analyzer = new AnalyzeObject(item, wrapChildren, wrapRealChildren, wrapperFactory);
        }
        protected readonly DynamicWrapperFactory WrapperFactory;
        //protected readonly object UnwrappedObject;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Analyzer.TryGet(binder.Name).Result;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) 
            => throw new NotSupportedException($"Setting a value on {nameof(DynamicReadObject)} is not supported");


        /// <inheritdoc />
        [PrivateApi("Internal")]
        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path) 
            => Analyzer.FindPropertyInternal(specs, path);


        object IHasJsonSource.JsonSource => Analyzer.GetContents();

        public dynamic Get(string name) => Analyzer.TryGet(name).Result;

        [PrivateApi]
        public List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path) => Analyzer._Dump(specs, path);

    }
}