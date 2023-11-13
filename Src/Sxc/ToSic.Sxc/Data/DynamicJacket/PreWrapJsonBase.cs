using ToSic.Eav.Data.PropertyLookup;
using ToSic.Sxc.Data.Wrapper;

namespace ToSic.Sxc.Data
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal abstract class PreWrapJsonBase: PreWrapBase, IPreWrap, IPropertyLookup
    {
        internal PreWrapJsonBase(CodeJsonWrapper wrapper, object data): base(data)
        {
            Wrapper = wrapper;
        }

        public readonly CodeJsonWrapper Wrapper;

        public override WrapperSettings Settings => Wrapper.Settings;
        
    }
}
