using ToSic.Eav.Data.PropertyLookup;
using ToSic.Sxc.Data.Wrapper;

namespace ToSic.Sxc.Data
{
    internal abstract class PreWrapJsonBase: PreWrapBase, IPreWrap, IPropertyLookup //, IHasKeys
    {
        internal PreWrapJsonBase(CodeJsonWrapper wrapper)
        {
            Wrapper = wrapper;
        }

        public readonly CodeJsonWrapper Wrapper;

        public override WrapperSettings Settings => Wrapper.Settings;

        //public bool IsNotEmpty(string name, string noParamOrder = Protector)
        //    => HasKeysHelper.IsNotEmpty(Get(name), null);

        //public bool IsEmpty(string name, string noParamOrder = Protector) 
        //    => HasKeysHelper.IsEmpty(Get(name), null);
    }
}
