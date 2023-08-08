namespace ToSic.Sxc.Data.Wrapper
{
    public class ReWrapSettings
    {
        private ReWrapSettings(bool children, bool realObjectsToo, bool wrapDynamic)
        {
            Children = children;
            RealObjectsToo = realObjectsToo;
            WrapDynamic = wrapDynamic;
        }

        public static ReWrapSettings Dyn(bool children, bool realObjectsToo)
            => new ReWrapSettings(children, realObjectsToo, true);

        public static ReWrapSettings Typed(bool children, bool realObjectsToo)
            => new ReWrapSettings(children, realObjectsToo, false);

        /// <summary>
        /// Determine if children of this object should be re wrapped into special objects,
        /// IF they are themselves richer objects (classes, anonymous); but not values.
        /// </summary>
        public bool Children { get; }

        public bool RealObjectsToo { get; }

        /// <summary>
        /// Only re-wrap anonymous child objects.
        /// This means that it should skip "real" objects with a type.
        /// </summary>
        public bool AnonymousOnly => !RealObjectsToo;

        public bool WrapDynamic { get; }
    }
}
