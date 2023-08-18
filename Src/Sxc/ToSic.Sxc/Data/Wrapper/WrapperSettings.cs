namespace ToSic.Sxc.Data.Wrapper
{
    public readonly struct WrapperSettings
    {
        private WrapperSettings(bool wrapChildren, bool wrapRealObjects, bool wrapToDynamic, bool strict = false)
        {
            WrapChildren = wrapChildren;
            WrapRealObjects = wrapRealObjects;
            WrapToDynamic = wrapToDynamic;
            GetStrict = strict;
        }

        public static WrapperSettings Dyn(bool children, bool realObjectsToo, bool strict = false)
            => new WrapperSettings(wrapChildren: children, wrapRealObjects: realObjectsToo, wrapToDynamic: true, strict: strict);

        public static WrapperSettings Typed(bool children, bool realObjectsToo, bool strict = true)
            => new WrapperSettings(wrapChildren: children, wrapRealObjects: realObjectsToo, wrapToDynamic: false, strict: strict);

        /// <summary>
        /// Determine if children of this object should be re wrapped into special objects,
        /// IF they are themselves richer objects (classes, anonymous); but not values.
        /// </summary>
        public bool WrapChildren { get; }

        /// <summary>
        /// Determine if real objects (with an existing class, eg. non Anonymous) should also be wrapped.
        /// </summary>
        public bool WrapRealObjects { get; }

        /// <summary>
        /// Determines if wrapping should result in a Dynamic or typed object.
        /// </summary>
        public bool WrapToDynamic { get; }

        /// <summary>
        /// Determine if Get will be strict
        /// </summary>
        public bool GetStrict { get; }
    }
}
