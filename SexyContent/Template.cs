using System.Data;
using System.Data.Objects.DataClasses;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.UI;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.SexyContent;
using System;
using DotNetNuke.UI.Modules;
using DotNetNuke.Entities.Portals;
using System.Web;
using System.Reflection;
using ToSic.SexyContent.Engines;
using System.Collections.Generic;
using System.Linq;

namespace ToSic.SexyContent
{
    public partial class Template
    {

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        public bool IsRazor
        {
            get
            {
                return (Type == "C# Razor" || Type == "VB Razor");
            }
        }

    }
}