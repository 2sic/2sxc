using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using DotNetNuke.Entities.Modules;
using ToSic.SexyContent;

namespace ToSic.SexyContent
{
    public partial class Settings : DotNetNuke.Entities.Modules.ModuleSettingsBase
    {
        /// <summary>
        /// Gets called from DotNetNuke
        /// Prepares and fills the controls according to module settings.
        /// </summary>
        public override void LoadSettings()
        {
            // Nothing to do if PostBack
            if (Page.IsPostBack)
                return;

            // Load DataSource Settings
            chkPublishSource.Checked = Settings.ContainsKey(SexyContent.SettingsPublishDataSource) && Boolean.Parse(Settings[SexyContent.SettingsPublishDataSource].ToString());
            txtPublishStreams.Text = Settings.ContainsKey(SexyContent.SettingsPublishDataSourceStreams) ? Settings[SexyContent.SettingsPublishDataSourceStreams].ToString() : "Default,ListContent";
        }

        /// <summary>
        /// Gets called from DotNetNuke or the SettingsWrapper control.
        /// Saves the new settings.
        /// </summary>
        public override void UpdateSettings()
        {
            var moduleController = new ModuleController();

            // Save DataSource Settings
            moduleController.UpdateModuleSetting(ModuleId, SexyContent.SettingsPublishDataSource, chkPublishSource.Checked.ToString());
            moduleController.UpdateModuleSetting(ModuleId, SexyContent.SettingsPublishDataSourceStreams, txtPublishStreams.Text);
        }

        protected string GetJsonUrl()
        {
            var url = DotNetNuke.Common.Globals.NavigateURL(this.TabId);
            url += (url.Contains("?") ? "&" : "?") + "mid=" + ModuleId.ToString() +
                   "&standalone=true&type=data&popUp=true";
            return url;
        }
    }
}