using ToSic.Oqt.Coding;
using ToSic.Sxc.Oqt.Shared.Interfaces;

namespace ToSic.Sxc.Oqt.Client.Services;

/// <inheritdoc />
internal class OqtTurnOnService : IOqtTurnOnService
{
    public string Run(object runOrSpecs, NoParamOrderOqtane noParamOrder = default, object? require = null, object? data = null,
        IEnumerable<object>? args = default, string? addContext = default)
    {
        throw new System.NotImplementedException();
    }
}