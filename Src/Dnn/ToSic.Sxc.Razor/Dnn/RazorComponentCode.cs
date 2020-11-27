using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;
using ToSic.Sxc.Run.Context;
using ToSic.Sxc.Search;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is the type used by code-behind classes of razor components.
    /// Use it to move logic / functions etc. into a kind of code-behind razor instead of as part of your view-template.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class RazorComponentCode: RazorComponent
    {
        ///// <summary>
        ///// This is called before rendering.
        ///// CustomizeData has already happened at this moment.
        ///// Override this method to also run any code automatically before rendering. <br/>
        ///// </summary>
        //public virtual void OnRender() { }

        ///// <summary>
        ///// Override this method to also run any code automatically after rendering. <br/>
        ///// It's meant for things like setting page headers etc. <br/>
        ///// Note that it's run at the end of the render-cycle.
        ///// </summary>
        //public virtual void OnRendered() { }


        /// <inheritdoc />
        public override void CustomizeData() { }

        /// <inheritdoc />
        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo,
            DateTime beginDate) { }

    }
}
