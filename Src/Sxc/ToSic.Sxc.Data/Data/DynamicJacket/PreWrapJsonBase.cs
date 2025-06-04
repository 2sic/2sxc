using ToSic.Eav.Data.Sys;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal abstract class PreWrapJsonBase(CodeJsonWrapper wrapper, object data)
    : PreWrapBase(data), IPreWrap, IPropertyLookup
{
    public readonly CodeJsonWrapper Wrapper = wrapper;

    public override WrapperSettings Settings => Wrapper.Settings;
        
}