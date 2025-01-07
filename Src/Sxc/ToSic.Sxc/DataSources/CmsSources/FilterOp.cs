namespace ToSic.Sxc.DataSources;

internal record FilterOp<T>(string Name, Func<T, bool> Filter);