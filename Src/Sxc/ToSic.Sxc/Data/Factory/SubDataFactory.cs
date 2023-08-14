using ToSic.Eav.Data;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This helps create sub-items for a specific context, obeying the rules of the context
    /// </summary>
    internal class SubDataFactory
    {
        public CodeDataFactory Cdf { get; }
        public bool Strict { get; }
        public bool? Debug { get; }

        public SubDataFactory(CodeDataFactory cdf, bool strict, bool? debug)
        {
            Cdf = cdf;
            Strict = strict;
            Debug = debug;
        }

        /// <summary>
        /// Generate a dynamic entity based on an IEntity.
        /// Used in various cases where a property would return an IEntity, and the Razor should be able to continue in dynamic syntax
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public IDynamicEntity SubDynEntityOrNull(IEntity contents) => SubDynEntityOrNull(contents, Cdf, Debug, strictGet: Strict);

        internal static DynamicEntity SubDynEntityOrNull(IEntity contents, CodeDataFactory cdf, bool? debug, bool strictGet)
        {
            if (contents == null) return null;
            var result = cdf.AsDynamic(contents, strictGet); // new DynamicEntity(contents, cdf, strict: strictGet);
            if (debug == true) result.Debug = true;
            return result;
        }

    }
}
