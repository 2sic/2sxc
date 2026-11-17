using System.Threading;

namespace ToSic.Sxc.Oqt.Server.Context;

/// <summary>Identifies the tenant of an explicitly scoped background operation.</summary>
/// <remarks>
/// This carries cache identity across awaits. The caller must also initialize Oqtane's
/// scoped tenant manager; this marker does not select a database connection by itself.
/// </remarks>
internal sealed class OqtBackgroundTenantScope : IDisposable
{
    // The shared app cache outlives DI scopes. Carry only an immutable tenant id
    // through the execution context, never a scoped service or HttpContext.
    private static readonly AsyncLocal<int?> Current = new();
    private readonly int? _previous;

    public OqtBackgroundTenantScope(int tenantId)
    {
        _previous = Current.Value;
        Current.Value = tenantId;
    }

    internal static int? TenantId => Current.Value;

    // Restore the outer operation/request even when a nested indexing operation fails.
    public void Dispose() => Current.Value = _previous;
}
