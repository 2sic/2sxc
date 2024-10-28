namespace ToSic.Sxc.Data.Internal.Wrapper;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public readonly struct WrapperSettings
{
    private WrapperSettings(bool wrapChildren, bool wrapRealObjects, bool wrapToDynamic, bool propsRequired = false)
    {
        WrapChildren = wrapChildren;
        WrapRealObjects = wrapRealObjects;
        WrapToDynamic = wrapToDynamic;
        PropsRequired = propsRequired;
    }

    public static WrapperSettings Dyn(bool children, bool realObjectsToo, bool propsRequired = false)
        => new(wrapChildren: children, wrapRealObjects: realObjectsToo, wrapToDynamic: true, propsRequired: propsRequired);

    public static WrapperSettings Typed(bool children, bool realObjectsToo, bool propsRequired = true)
        => new(wrapChildren: children, wrapRealObjects: realObjectsToo, wrapToDynamic: false, propsRequired: propsRequired);

    /// <summary>
    /// Determine if children of this object should be re wrapped into special objects,
    /// IF they are themselves richer objects (classes, anonymous); but not values.
    /// </summary>
    public bool WrapChildren { get; }

    /// <summary>
    /// Determine if real objects (with an existing class, like non-Anonymous) should also be wrapped.
    /// </summary>
    public bool WrapRealObjects { get; }

    /// <summary>
    /// Determines if wrapping should result in a Dynamic or typed object.
    /// </summary>
    public bool WrapToDynamic { get; }

    /// <summary>
    /// Determine if Get will be strict
    /// </summary>
    public bool PropsRequired { get; }
}