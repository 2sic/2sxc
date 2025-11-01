using ToSic.Eav.Data.Sys.PropertyLookup;
using ToSic.Sxc.Data.Sys.Wrappers;

namespace ToSic.Sxc.Data.Sys.DynamicJacket;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal abstract class PreWrapJsonBase(CodeJsonWrapper wrapper, object data)
    : PreWrapBase(data), IPreWrap, IPropertyLookup
{
    public readonly CodeJsonWrapper Wrapper = wrapper;

    public override WrapperSettings Settings => Wrapper.Settings;
        
}