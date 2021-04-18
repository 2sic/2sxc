using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Hybrid
{
    [PrivateApi("not sure yet if this will stay in Hybrid or go to Web.Razor or something, so keep it private for now")]
    public interface IHybridRazorComponent: IDynamicCode
    {
        /// <summary>
        /// For Razor components we need to ensure that we can set/update the _DynCodeRoot
        /// </summary>
        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        new IDynamicCodeRoot _DynCodeRoot { set; }

        //Purpose Purpose { get; set; }

    }
}
