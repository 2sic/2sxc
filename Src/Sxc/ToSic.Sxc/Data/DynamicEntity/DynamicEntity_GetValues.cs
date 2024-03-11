using System.Dynamic;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data.Internal.Dynamic;

namespace ToSic.Sxc.Data;

public partial class DynamicEntity
{
    [PrivateApi]
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        // Check special cases #1 Toolbar - only in DNN and only on the explicit dynamic entity; not available in Oqtane
#if NETFRAMEWORK
#pragma warning disable 618 // ignore Obsolete
        if (binder.Name == ViewConstants.FieldToolbar)
        {
            result = Toolbar.ToString();
            return true;
        }
#pragma warning restore 618
#endif

        return CodeDynHelper.TryGetMemberAndRespectStrict(GetHelper, binder, out result);
    }
        
}