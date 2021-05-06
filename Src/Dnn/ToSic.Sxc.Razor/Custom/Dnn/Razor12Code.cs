using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Search;

// ReSharper disable once CheckNamespace
namespace Custom.Dnn
{
    /// <summary>
    /// This is the type used by code-behind classes of razor components in 2sxc v12+
    /// Use it to move logic / functions etc. into a kind of code-behind razor instead of as part of your view-template.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class Razor12Code: RazorComponent
    {

        /// <inheritdoc />
        public override void CustomizeData() { }

#pragma warning disable 618
        /// <inheritdoc />
        [PrivateApi]
        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate)
        {
            CustomizeSearch(searchInfos, moduleInfo as IContainer, beginDate);
        }

        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo, DateTime beginDate) { }
#pragma warning restore 618

    }
}
