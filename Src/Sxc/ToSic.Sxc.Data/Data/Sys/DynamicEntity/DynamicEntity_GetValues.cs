﻿using System.Dynamic;
using ToSic.Sxc.Data.Sys.Dynamic;

namespace ToSic.Sxc.Data.Sys;

public partial class DynamicEntity
{
    [PrivateApi]
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        // Check special cases #1 Toolbar - only in DNN and only on the explicit dynamic entity; not available in Oqtane
#if NETFRAMEWORK
#pragma warning disable 618 // ignore Obsolete
        if (binder.Name == nameof(Toolbar))
        {
            result = Toolbar.ToString();
            return true;
        }
#pragma warning restore 618
#endif

        return CodeDynHelper.TryGetMemberAndRespectStrict(GetHelper, binder, out result);
    }
        
}