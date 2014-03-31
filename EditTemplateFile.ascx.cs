using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using DotNetNuke.Web.UI.WebControls;

namespace ToSic.SexyContent
{
    public partial class EditTemplateFile : SexyControlEditBase
    {

        private const string AdditionalSystemTokens = "[Content:Toolbar],ContentToolbar";
        private const string ListAdditionalSystemTokens = "[List:Index],ListIndex;[List:Index1],ListIndex1;[List:Count],ListCount;[List:IsFirst],ListIsFirst;[List:IsLast],ListIsLast;[List:Alternator2],ListAlternator2;[List:Alternator3],ListAlternator3;[List:Alternator4],ListAlternator4;[List:Alternator5],ListAlternator5;[ListContent:Toolbar],ListToolbar;<repeat>...</repeat>,ListRepeat";

        private const string AdditionalSystemRazor = "@Content.Toolbar,ContentToolbar";
        private const string ListAdditionalSystemRazor = "@ListContent.Toolbar,ListToolbar";

        private bool UserMayEdit
        {
            get
            {
                return !UserInfo.IsSuperUser && (Template.Location == SexyContent.TemplateLocations.HostFileSystem || Template.IsRazor);
            }
        }

        private Template Template
        {
            get {
                int TemplateID = int.Parse(Request.QueryString["TemplateID"]);
                return Sexy.TemplateContext.GetTemplate(TemplateID);
            }
        }

        private string TemplatePath
        {
            get {
                return Server.MapPath(System.IO.Path.Combine(Sexy.GetTemplatePathRoot(Template.Location), Template.Path));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserMayEdit)
            {
                btnUpdate.Visible = false;
                txtTemplateContent.Enabled = false;
            }

            hlkCancel.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(this.TabId);

            if (IsPostBack)
                return;

            if (Template == null)
                return;

            if (File.Exists(TemplatePath))
                txtTemplateContent.Text = File.ReadAllText(TemplatePath);

            // Get name of the current template and write it to lblTemplate
            lblTemplate.Text = Template.Name;
            lblTemplateLocation.Text = @"/" + TemplatePath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, String.Empty).Replace('\\','/');
            lblEditTemplateFileHeading.Text = String.Format(LocalizeString("lblEditTemplateFileHeading.Text"), Template.Name);


            var DefaultLanguageID = Sexy.ContentContext.GetLanguageId(PortalSettings.DefaultLanguage);
            var LanguageList = DefaultLanguageID.HasValue ? new[] {DefaultLanguageID.Value} : new[] { 0 };
            
            var TemplateDefaults = Sexy.GetTemplateDefaults(Template.TemplateID).Where(t => t.ContentTypeID.HasValue);
            string FormatString;

            if (Template.Type == "Token")
                FormatString = "[{0}:{1}]";
            else
                FormatString = "@{0}.{1}";

            foreach(var TemplateDefault in TemplateDefaults)
            {
                ToSic.Eav.AttributeSet Set = Sexy.ContentContext.GetAttributeSet(TemplateDefault.ContentTypeID.Value);
                
                var DataSource = Sexy.ContentContext.GetAttributes(Set, true).Select(a => new {
                    StaticName = String.Format(FormatString, TemplateDefault.ItemType.ToString("F"), a.StaticName),
                    DisplayName = (Sexy.ContentContext.GetAttributeMetaData(a.AttributesInSets.FirstOrDefault().AttributeID)).ContainsKey("Name") ? (Sexy.ContentContext.GetAttributeMetaData(a.AttributesInSets.FirstOrDefault().AttributeID))["Name"][LanguageList] : a.StaticName + " (static)"
                }).ToList();

                AddFieldGrid(DataSource, TemplateDefault.ItemType.ToString("F"));
            }

            
            if (!Template.IsRazor)
            {
                AddFieldGrid(AdditionalSystemTokens.Split(';').Select(d => new { StaticName = d.Split(',')[0], DisplayName = LocalizeString(d.Split(',')[1] + ".Text")}), "System");

                if (Template.UseForList)
                    AddFieldGrid(ListAdditionalSystemTokens.Split(';').Select(d => new { StaticName = HttpUtility.HtmlEncode(d.Split(',')[0]), DisplayName = LocalizeString(d.Split(',')[1] + ".Text") }), "ListSystem");
            }
            else
            {
                AddFieldGrid(AdditionalSystemRazor.Split(';').Select(d => new { StaticName = d.Split(',')[0], DisplayName = LocalizeString(d.Split(',')[1] + ".Text") }), "System");

                if(Template.UseForList)
                    AddFieldGrid(ListAdditionalSystemRazor.Split(';').Select(d => new { StaticName = d.Split(',')[0], DisplayName = LocalizeString(d.Split(',')[1] + ".Text") }), "ListSystem");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if(!UserMayEdit)
                return;

            if (File.Exists(TemplatePath))
            {
                File.WriteAllText(TemplatePath, txtTemplateContent.Text);
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(this.TabId));
            }
        }

        protected void AddFieldGrid(object DataSource, string HeaderText)
        {
            var GridControl = (TemplateHelpGrid)LoadControl(Path.Combine(TemplateSourceDirectory, "TemplateHelpGrid.ascx"));
            var Grid = GridControl.Grid;


            // DataBind the GridView with the Tokens
            DnnGridBoundColumn TokenColumn = ((DnnGridBoundColumn)Grid.Columns.FindByUniqueName("StaticName"));
            TokenColumn.HeaderText = HeaderText;
            
            Grid.DataSource = DataSource;
            Grid.DataBind();
            
            phGrids.Controls.Add(GridControl);
        }
    }
}