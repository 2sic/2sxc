using System.Runtime.CompilerServices;

namespace ToSic.Sxc.DataSources;

internal record FilterOp<T>(
    Func<T, bool> Filter,
    [CallerMemberName] string Name = default
    );