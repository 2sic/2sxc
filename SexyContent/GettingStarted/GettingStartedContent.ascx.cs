using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Framework;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent.GettingStarted
{
    public partial class GettingStartedContent : SexyControlEditBase
    {
        public int ModuleID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
        }

    }
}