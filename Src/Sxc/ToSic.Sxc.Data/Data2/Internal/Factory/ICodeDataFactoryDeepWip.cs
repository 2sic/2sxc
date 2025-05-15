using ToSic.Eav.Apps;

namespace ToSic.Sxc.Data.Internal;

/// <summary>
/// Temporary interface so that `DynamicEntity` and `Field` can access specific data without
/// needing to know the IBlockContext, which should not be available in those APIs
/// </summary>
public interface ICodeDataFactoryDeepWip
{
    IAppReader AppReaderOrNull { get; }

    int AppIdOrZero { get; }
}