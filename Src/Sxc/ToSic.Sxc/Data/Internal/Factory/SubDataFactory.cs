using ToSic.Eav.Data;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data.Internal;

/// <summary>
/// This helps create sub-items for a specific context, obeying the rules of the context
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class SubDataFactory
{
    private readonly ICanDebug _canDebug;
    public Internal.CodeDataFactory Cdf { get; }
    public bool PropsRequired { get; }

    public SubDataFactory(Internal.CodeDataFactory cdf, bool propsRequired, ICanDebug canDebug)
    {
        _canDebug = canDebug;
        Cdf = cdf;
        PropsRequired = propsRequired;
    }

    /// <summary>
    /// Generate a dynamic entity based on an IEntity.
    /// Used in various cases where a property would return an IEntity, and the Razor should be able to continue in dynamic syntax
    /// </summary>
    /// <param name="contents"></param>
    /// <returns></returns>
    public IDynamicEntity SubDynEntityOrNull(IEntity contents) => SubDynEntityOrNull(contents, Cdf, _canDebug.Debug, propsRequired: PropsRequired);

    internal static DynamicEntity SubDynEntityOrNull(IEntity contents, Internal.CodeDataFactory cdf, bool? debug, bool propsRequired)
    {
        if (contents == null) return null;
        var result = cdf.AsDynamic(contents, propsRequired);
        if (debug == true) result.Debug = true;
        return result;
    }

}