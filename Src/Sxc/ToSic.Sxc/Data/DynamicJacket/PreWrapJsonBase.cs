using ToSic.Eav.Data.PropertyLookup;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal abstract class PreWrapJsonBase(CodeJsonWrapper wrapper, object data)
    : PreWrapBase(data), IPreWrap, IPropertyLookup
{
    public readonly CodeJsonWrapper Wrapper = wrapper;

    public override WrapperSettings Settings => Wrapper.Settings;
        
}