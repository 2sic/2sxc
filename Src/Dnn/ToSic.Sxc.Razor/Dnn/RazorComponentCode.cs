using ToSic.Eav.Documentation;
using Custom.Dnn;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is the type used by code-behind classes of razor components.
    /// Use it to move logic / functions etc. into a kind of code-behind razor instead of as part of your view-template.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class RazorComponentCode: Razor12Code
    {

//        /// <inheritdoc />
//        public override void CustomizeData() { }

//#pragma warning disable 618
//        /// <inheritdoc />
//        [PrivateApi]
//        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate)
//        {
//            CustomizeSearch(searchInfos, moduleInfo as IContainer, beginDate);
//        }

//        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo, DateTime beginDate) { }
//#pragma warning restore 618

    }
}
