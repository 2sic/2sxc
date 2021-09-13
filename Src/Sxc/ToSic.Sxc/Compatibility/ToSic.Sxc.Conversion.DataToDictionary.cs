#if NETFRAMEWORK
using System;
using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Conversion
{
    /// <summary>
    /// Old implementation with simple constructor. Shouldn't be used any more, because it needs DI now. 
    /// </summary>
    [PrivateApi("Hide implementation")]
    [Obsolete]
    public class DataToDictionary: Data.ConvertToEavLightWithCmsInfo
    {

        /// <summary>
        /// Old constructor, for old use cases. Was published in tutorial for a while; not ideal...
        /// </summary>
        [PrivateApi]
        [Obsolete("only keep in case external code was using this in apps ca. 2sxc 11. v12+ should use GetService")]
	    public DataToDictionary(): base(Eav.Factory.ObsoleteBuild<Dependencies>()) { }

        /// <summary>
        /// Old constructor, for old use cases. Was published in tutorial for a while; not ideal...
        /// </summary>
        /// <param name="withEdit">Include editing information in serialized result</param>
        ///// <param name="languages"></param>
        [Obsolete("only keep in case external code was using this in apps ca. 2sxc 11. v12+ should use GetService")]
        public DataToDictionary(bool withEdit) : this() => WithEdit = withEdit;
    }
}
#endif
