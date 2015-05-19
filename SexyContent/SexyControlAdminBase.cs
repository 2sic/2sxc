using System;
using DotNetNuke.UI.Modules;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Contains properties that all controls use that edit the current module's data (not global data like admin controls)
    /// It delivers a context that uses the current modules App and the current portal's Zone.
    /// </summary>
    public abstract class SexyControlAdminBase : SexyControlEditBase
    {

        protected override int? AppId
        {
            get
            {
                var appIdString = Request.QueryString[SexyContent.AppIDString];
                int appId;
                if (appIdString != null && int.TryParse(appIdString, out appId))
                    return appId;

                return null;
            }
        }

    }
}