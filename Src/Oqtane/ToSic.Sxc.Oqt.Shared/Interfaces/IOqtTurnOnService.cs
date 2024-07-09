using System.Collections.Generic;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

/// <summary>
/// turnOn Service helps initialize / boot JavaScripts when all requirements (usually dependencies) are ready.
/// </summary>
[PrivateApi("Don't publish yet - the functionality is surfaced on the PageService!")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IOqtTurnOnService 
{
    string Run(object runOrSpecs,
        NoParamOrder noParamOrder = default,
        object require = null,
        object data = null,
        IEnumerable<object> args = default,
        string addContext = default
    );
}