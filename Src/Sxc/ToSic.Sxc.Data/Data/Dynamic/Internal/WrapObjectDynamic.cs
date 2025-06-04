using System.Dynamic;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Data.Sys;
using ToSic.Lib.Data;
using ToSic.Lib.Wrappers;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data.Internal.Dynamic;

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
[ShowApiWhenReleased(ShowApiMode.Never)]
public class WrapObjectDynamic: DynamicObject, IWrapper<object>, IPropertyLookup, IHasJsonSource, ICanGetByName
{
    [PrivateApi]
    public object GetContents() => ((IWrapper<object>)PreWrap).GetContents();

    [PrivateApi]
    internal readonly PreWrapObject PreWrap;

    [PrivateApi]
    internal WrapObjectDynamic(PreWrapObject preWrap, ICodeDataPoCoWrapperService wrapperSvc)
    {
        WrapperSvc = wrapperSvc;
        PreWrap = preWrap;
    }
    protected readonly ICodeDataPoCoWrapperService WrapperSvc;

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


    object IHasJsonSource.JsonSource()
        => ((IWrapper<object>)PreWrap).GetContents();

    public dynamic Get(string name)
        => PreWrap.TryGetWrap(name, true).Result;

    // #DropUseOfDumpProperties
    //[PrivateApi]
    //public List<PropertyDumpItem> _DumpNameWipDroppingMostCases(PropReqSpecs specs, string path)
    //    => PreWrap._DumpNameWipDroppingMostCases(specs, path);

}