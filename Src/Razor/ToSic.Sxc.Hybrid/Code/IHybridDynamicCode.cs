using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Hybrid.Code
{
    /// <summary>
    /// This interface extends the IAppAndDataHelpers with the Oqt Context.
    /// It's important, because if 2sxc also runs on other CMS platforms, then the Oqt Context won't be available, so it's in a separate interface.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface IHybridDynamicCode : Sxc.Code.IDynamicCode
    {
        /// <summary>
        /// The Oqt context.
        /// </summary>
        /// <returns>
        /// The Oqt context.
        /// </returns>
        // TODO: WIP
        //IOqtContext Oqt { get; }

    }
}
