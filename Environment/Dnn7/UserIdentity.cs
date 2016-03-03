using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using Telerik.Web.UI.PivotGrid.Core.Fields;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class UserIdentity
    {
        public static string CurrentUserIdentityToken
        {
            get
            {
                var userId = PortalSettings.Current?.UserId;
                var token = ((userId ?? -1) == -1) ? "anonymous" : "dnn:userid=" + userId;
                return token;


            }
        }
    }
}