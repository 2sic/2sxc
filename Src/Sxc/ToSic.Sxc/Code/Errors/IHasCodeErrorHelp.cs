using System.Collections.Generic;

namespace ToSic.Sxc.Code.Errors
{
    public interface IHasCodeErrorHelp
    {
        List<CodeHelp> ErrorHelpers { get; }
    }
}
