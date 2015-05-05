using System;
using System.IO;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Web.UI.WebControls;

namespace ToSic.SexyContent
{
    public partial class EditTemplateFile : SexyControlEditBase
    {

		//private const string AdditionalSystemTokens = "[Content:Toolbar],ContentToolbar";
		//private const string ListAdditionalSystemTokens = "[List:Index],ListIndex;[List:Index1],ListIndex1;[List:Count],ListCount;[List:IsFirst],ListIsFirst;[List:IsLast],ListIsLast;[List:Alternator2],ListAlternator2;[List:Alternator3],ListAlternator3;[List:Alternator4],ListAlternator4;[List:Alternator5],ListAlternator5;[ListContent:Toolbar],ListToolbar;<repeat>...</repeat>,ListRepeat";

		//private const string AdditionalSystemRazor = "@Content.Toolbar,ContentToolbar";
		//private const string ListAdditionalSystemRazor = "@ListContent.Toolbar,ListToolbar";

        private bool UserMayEdit
        {
            get
            {
                return UserInfo.IsSuperUser ||
                    (Template.Location == SexyContent.TemplateLocations.PortalFileSystem && !Template.IsRazor && UserInfo.IsInRole(PortalSettings.AdministratorRoleName));
            }
        }

        private Template Template
        {
            get {
                var TemplateID = int.Parse(Request.QueryString["TemplateID"]);
                return Sexy.Templates.GetTemplate(TemplateID);
            }
        }

        private string TemplatePath
        {
            get {
                return Server.MapPath(Path.Combine(SexyContent.GetTemplatePathRoot(Template.Location, Sexy.App), Template.Path));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserMayEdit)
            {
                btnUpdate.Visible = false;
                txtTemplateContent.Enabled = false;
            }

            hlkCancel.NavigateUrl = Globals.NavigateURL(TabId);

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


            var defaultLanguageID = Sexy.ContentContext.GetLanguageId(PortalSettings.DefaultLanguage);
            var languageList = defaultLanguageID.HasValue ? new[] {defaultLanguageID.Value} : new[] { 0 };
            
            //var templateDefaults = Sexy.GetTemplateDefaults(Template.TemplateID).Where(t => t.ContentTypeID.HasValue);
            string formatString;

            if (Template.Type == "Token")
                formatString = "[{0}:{1}]";
            else
                formatString = "@{0}.{1}";

	        AddHelpForAContentType(ContentGroup.Template.ContentTypeStaticName, formatString, "Content", languageList);
			AddHelpForAContentType(ContentGroup.Template.PresentationTypeStaticName, formatString.Replace("[{0}", "[Content:Presentation"), "Presentation", languageList);
			AddHelpForAContentType(ContentGroup.Template.ListContentTypeStaticName, formatString, "ListContent", languageList);
			AddHelpForAContentType(ContentGroup.Template.ListPresentationTypeStaticName, formatString.Replace("[{0}", "[ListContent:Presentation"), "ListPresentation", languageList);

			// todo: add AppResources and AppSettings help

			// add standard help
	        AddHelpFragments();
        }

		/// <summary>
		/// Create a help-table showing all the tokens/placeholders for a specific content type
		/// </summary>
		/// <param name="contentTypeId"></param>
		/// <param name="formatString"></param>
		/// <param name="TemplateDefault"></param>
		/// <param name="LanguageList"></param>
		private void AddHelpForAContentType(string contentTypeStaticName, string formatString, string itemType,
			int[] LanguageList)
		{
			if (String.IsNullOrEmpty(contentTypeStaticName))
				return;

			var set = Sexy.ContentContext.GetAttributeSet(contentTypeStaticName);

			var dataSource = Sexy.ContentContext.GetAttributes(set, true).Select(a => new
			{
				StaticName = String.Format(formatString, itemType, a.StaticName),
				DisplayName =
					(Sexy.ContentContext.GetAttributeMetaData(a.AttributesInSets.FirstOrDefault().AttributeID)).ContainsKey("Name")
						? (Sexy.ContentContext.GetAttributeMetaData(a.AttributesInSets.FirstOrDefault().AttributeID))["Name"][LanguageList]
						: a.StaticName + " (static)"
			}).ToList();

			AddFieldGrid(dataSource, itemType);
		}

	    /// <summary>
		/// Add helper infos to the editor, common tokens, razor snippets etc.
		/// </summary>
	    private void AddHelpFragments()
	    {
			// 2014-07-18 2dm - new
		    var lookupKey = Template.IsRazor ? "Razor" : "Token";
		    var additionalHelpers = LocalizeString("Additional" + lookupKey + "Sets.Text");
		    if (!Template.UseForList)
			    additionalHelpers = additionalHelpers.Replace("List" + lookupKey + ",", "");
		    foreach (var helperSet in additionalHelpers.Split(','))
		    {
			    var setText = LocalizeString(helperSet + ".List");
			    var hasEncodedStuff = (setText.IndexOf("»") > 0);
			    var splitFilter1 = hasEncodedStuff ? new[] {"»\r\n", "»\n"} : new[] {"\r\n", "\n"};
			    var splitFilter2 = hasEncodedStuff ? new[] {"«"} : new[] {"="};
			    var data =
				    setText.Split(splitFilter1, StringSplitOptions.None)
					    .Select(
						    d =>
							    new
							    {
								    StaticName = d.Split(splitFilter2, StringSplitOptions.None)[0],
								    DisplayName = d.Split(splitFilter2, StringSplitOptions.None)[1]
							    })
					    .ToList();
			    if (hasEncodedStuff)
				    data =
					    data.Select(x => new {StaticName = EncodeCode(x.StaticName), DisplayName = EncodeComment(x.DisplayName)}).ToList();
			    // todo: bug - cannot pass in translated title, the grid always tries to re-look it up in another resx
			    //var title = LocalizeString(tokenSet + ".Title");
			    AddFieldGrid(data, helperSet);
		    }


		    //// Add Token Help Tables
		    //if (!Template.IsRazor)
		    //{
		    //	// 2014-07-18 2dm - removed
		    //	// AddFieldGrid(AdditionalSystemTokens.Split(';').Select(d => new { StaticName = d.Split(',')[0], DisplayName = LocalizeString(d.Split(',')[1] + ".Text")}), "System");
		    //	//if (Template.UseForList)
		    //	//	AddFieldGrid(ListAdditionalSystemTokens.Split(';').Select(d => new { StaticName = HttpUtility.HtmlEncode(d.Split(',')[0]), DisplayName = LocalizeString(d.Split(',')[1] + ".Text") }), "ListSystem");

		    //}
		    //// Add Razor Help Tables
		    //else
		    //{
		    //	AddFieldGrid(AdditionalSystemRazor.Split(';').Select(d => new { StaticName = d.Split(',')[0], DisplayName = LocalizeString(d.Split(',')[1] + ".Text") }), "System");

		    //	if(Template.UseForList)
		    //		AddFieldGrid(ListAdditionalSystemRazor.Split(';').Select(d => new { StaticName = d.Split(',')[0], DisplayName = LocalizeString(d.Split(',')[1] + ".Text") }), "ListSystem");
		    //}
	    }

	    protected string EncodeCode(string original)
	    {
		    //return "<pre>" + original + "</pre>";
		    return Server.HtmlEncode(original)
			    .Replace("\r\n", "<br/>")
			    .Replace("\t", "&nbsp;");
	    }

	    protected string EncodeComment(string original)
	    {
			return Server.HtmlEncode(original)
				.Replace("\r\n", "<br/>")
				.Replace("\t", "&nbsp;");
		}

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if(!UserMayEdit)
                return;

            if (File.Exists(TemplatePath))
            {
                File.WriteAllText(TemplatePath, txtTemplateContent.Text);
                Response.Redirect(Globals.NavigateURL(TabId));
            }
        }

        protected void AddFieldGrid(object dataSource, string headerText)
        {
            var gridControl = (TemplateHelpGrid)LoadControl(Path.Combine(TemplateSourceDirectory, "TemplateHelpGrid.ascx"));
            var grid = gridControl.Grid;


            // DataBind the GridView with the Tokens
            var tokenColumn = ((DnnGridBoundColumn)grid.Columns.FindByUniqueName("StaticName"));
            tokenColumn.HeaderText = headerText;
            
            grid.DataSource = dataSource;
            grid.DataBind();
            
            phGrids.Controls.Add(gridControl);
        }
    }
}