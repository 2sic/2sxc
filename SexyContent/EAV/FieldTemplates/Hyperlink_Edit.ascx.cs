using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using System.Web;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Localization;
using Telerik.Web.UI;
using Telerik.Web.UI.Editor.DialogControls;
using System.Linq;
using ToSic.SexyContent.EAV.FieldTemplates;

namespace ToSic.Eav.ManagementUI
{
	public partial class Hyperlink_EditCustom : FieldTemplateUserControl
	{
		#region Protected Fields/Controls
		protected DotNetNuke.UI.UserControls.LabelControl FieldLabel;
		#endregion

		#region Private Fields
		private ImageManagerDialogConfiguration _imageManagerConfiguration = new ImageManagerDialogConfiguration();
		private FileManagerDialogConfiguration _documentManagerConfiguration = new FileManagerDialogConfiguration();
		private EditorProvider _editorProvider;
		private Panel _dnnPageDropDownList;
		#endregion

		#region Properties
		protected DialogTypeEnum DefaultDialog
		{
			get { return GetMetaDataValue("DefaultDialog", DialogTypeEnum.None); }
		}

		protected bool ShowPagePicker
		{
			get { return GetMetaDataValue<bool>("ShowPagePicker") || DefaultDialog == DialogTypeEnum.PagePicker; }
		}

		protected bool ShowImageManager
		{
			get { return GetMetaDataValue<bool>("ShowImageManager") || DefaultDialog == DialogTypeEnum.ImageManager; }
		}

		protected bool ShowFileManager
		{
			get { return GetMetaDataValue<bool>("ShowFileManager") || DefaultDialog == DialogTypeEnum.FileManager; }
		}

		public override object Value
		{
			get { return txtFilePath.Text; }
		}

		public override Control DataControl
		{
			get { return txtFilePath; }
		}
		#endregion

		protected void Page_Init(object sender, EventArgs e)
		{
			if (ShowPagePicker)
				InitDnnPageDropDownList(PortalSettings.Current.PortalId);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			DataBind();

			if (DefaultDialog == DialogTypeEnum.None)
				hlkFileBrowse.Visible = false;

			if (ShowPagePicker)
				LoadDnnPageDropDownList(PortalSettings.Current.PortalId);
		}

		protected override void OnPreRender(EventArgs e)
		{
		    if (!IsPostBack)
		    {
                if(FieldValueEditString == null)
                    txtFilePath.Text = GetMetaDataValue("DefaultValue", "");
                else
		            txtFilePath.Text = FieldValueEditString;
		    }

		    InitDialogOpener();

			FieldLabel.Text = GetMetaDataValue("Name", Attribute.StaticName);
			FieldLabel.HelpText = GetMetaDataValue<string>("Notes");
            valFieldValue.Enabled = GetMetaDataValue<bool>("Required");

			base.OnPreRender(e);
		}

		#region Dialog Opener

		private void InitEditorProvider()
		{
			_editorProvider = new EditorProvider();

			_editorProvider.Initialize();
			_imageManagerConfiguration = _editorProvider._editor.ImageManager;
			_documentManagerConfiguration = _editorProvider._editor.DocumentManager;
		}

		private void InitDialogOpener()
		{
			InitEditorProvider();

			#region Get View, Update, DeletePaths
			var homeDirectory = PortalSettings.Current.HomeDirectory;
			string[] paths;
		    var metaDataPaths = GetMetaDataValue<string>("Paths");
			if (!String.IsNullOrEmpty(metaDataPaths))
                paths = metaDataPaths.Split(',').Select(p => homeDirectory + p).ToArray();
			else
				paths = _imageManagerConfiguration.ViewPaths;

			#endregion


			if (ShowImageManager)
			{
				DialogOpener1.DialogDefinitions.Add("ImageManager", GetImageManagerDefinition(paths));
				DialogOpener1.DialogDefinitions.Add("ImageEditor", GetImageEditorDefinition(paths));
			}
			if (ShowFileManager)
				DialogOpener1.DialogDefinitions.Add("DocumentManager", GetDocumentManagerDefinition(paths));

			DialogOpener1.HandlerUrl = "~/DesktopModules/Admin/RadEditorProvider/DialogHandler.aspx?portalid=" + PortalSettings.Current.PortalId + "&tabid=" + PortalSettings.Current.ActiveTab.TabID;
			if (!String.IsNullOrWhiteSpace(FieldValueEditString) && FieldValueEditString.StartsWith("File:"))
				DialogOpener1.AdditionalQueryString = "&PreselectedItemUrl=" + HttpUtility.UrlEncode(SexyContent.SexyContent.ResolveHyperlinkValues(FieldValueEditString, PortalSettings.Current));
			DialogOpener1.EnableEmbeddedSkins = _editorProvider._editor.EnableEmbeddedSkins;
			DialogOpener1.Skin = _editorProvider._editor.Skin;

			InitPickerMenu();
		}

		private void InitPickerMenu()
		{
			var visibleItems = 0;
			// Page Picker
			liPagePicker.Visible = ShowPagePicker;
			if (ShowPagePicker)
			{
				visibleItems++;
				liPagePicker.Attributes.Add("onclick", GetClientOpenDialogCommand(DialogTypeEnum.PagePicker));
			}

			// Document Manager
			liDocumentManager.Visible = ShowFileManager;
			if (ShowFileManager)
			{
				visibleItems++;
				liDocumentManager.Attributes.Add("onclick", GetClientOpenDialogCommand(DialogTypeEnum.FileManager));
			}

			// Image Manager
			liImageManager.Visible = ShowImageManager;
			if (ShowImageManager)
			{
				visibleItems++;
				liImageManager.Attributes.Add("onclick", GetClientOpenDialogCommand(DialogTypeEnum.ImageManager));
			}

			if (visibleItems <= 1)
				ulPickerMenu.Visible = false;
		}

		protected string GetClientOpenDialogCommand()
		{
			return GetClientOpenDialogCommand(DefaultDialog);
		}
		protected string GetClientOpenDialogCommand(DialogTypeEnum dialogType)
		{
			return "ToSexyContent.ItemForm.Hyperlink.OpenDialog(this, \"" + DialogOpener1.ClientID + "\", \"" + dialogType + "\", \"" + Attribute.StaticName + "\", " + PortalSettings.Current.PortalId + ", \"" + PortalSettings.Current.HomeDirectory + "\");";
		}

		private DialogDefinition GetImageManagerDefinition(string[] paths)
		{
			var imageManagerParameters = new FileManagerDialogParameters
			{
				ViewPaths = paths,
				UploadPaths = paths,
				DeletePaths = paths,
				MaxUploadFileSize = _imageManagerConfiguration.MaxUploadFileSize,
				FileBrowserContentProviderTypeName = _imageManagerConfiguration.ContentProviderTypeName,
				SearchPatterns = _imageManagerConfiguration.SearchPatterns,
			};
			imageManagerParameters["IsSkinTouch"] = false;
            if (!String.IsNullOrEmpty(GetMetaDataValue<string>("FileFilter")))
                imageManagerParameters.SearchPatterns = GetMetaDataValue<string>("FileFilter").Split(',');

			var imageManagerDefinition = new DialogDefinition(typeof(ImageManagerDialog), imageManagerParameters)
			{
				ClientCallbackFunction = "ToSexyContent.ItemForm.Hyperlink.ImageManagerCallback",
				Width = Unit.Pixel(694),
				Height = Unit.Pixel(440),
				Title = "Image Manager"
			};
			imageManagerDefinition.Parameters["Language"] = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			imageManagerDefinition.Parameters["LocalizationPath"] = "~/DesktopModules/Admin/RadEditorProvider/App_LocalResources/";

			return imageManagerDefinition;
		}

		private DialogDefinition GetDocumentManagerDefinition(string[] paths)
		{
			var documentManagerParameters = new FileManagerDialogParameters
			{
				ViewPaths = paths,
				UploadPaths = paths,
				DeletePaths = paths,
				MaxUploadFileSize = _documentManagerConfiguration.MaxUploadFileSize,
				FileBrowserContentProviderTypeName = _documentManagerConfiguration.ContentProviderTypeName,
				SearchPatterns = _documentManagerConfiguration.SearchPatterns
			};

			documentManagerParameters["IsSkinTouch"] = false;
            if (!String.IsNullOrEmpty(GetMetaDataValue<string>("FileFilter")))
                documentManagerParameters.SearchPatterns = GetMetaDataValue<string>("FileFilter").Split(',');

			var documentManagerDefinition = new DialogDefinition(typeof(DocumentManagerDialog), documentManagerParameters)
			{
				ClientCallbackFunction = "ToSexyContent.ItemForm.Hyperlink.DocumentManagerCallback",
				Width = Unit.Pixel(694),
				Height = Unit.Pixel(440),
				Title = "Document Manager"
			};
			documentManagerDefinition.Parameters["Language"] = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			documentManagerDefinition.Parameters["LocalizationPath"] = "~/DesktopModules/Admin/RadEditorProvider/App_LocalResources/";

			return documentManagerDefinition;
		}

		private DialogDefinition GetImageEditorDefinition(string[] paths)
		{
			var imageEditorParameters = new FileManagerDialogParameters
			{
				ViewPaths = paths,
				UploadPaths = paths,
				DeletePaths = paths,
				MaxUploadFileSize = _imageManagerConfiguration.MaxUploadFileSize,
				FileBrowserContentProviderTypeName = _imageManagerConfiguration.ContentProviderTypeName,
				SearchPatterns = _imageManagerConfiguration.SearchPatterns
			};
			imageEditorParameters["IsSkinTouch"] = false;
            if (!String.IsNullOrEmpty(GetMetaDataValue<string>("FileFilter")))
                imageEditorParameters.SearchPatterns = GetMetaDataValue<string>("FileFilter").Split(',');

			var imageEditorDefinition = new DialogDefinition(typeof(ImageEditorDialog), imageEditorParameters)
			{
				Width = Unit.Pixel(832),
				Height = Unit.Pixel(500),
				Title = "Image Editor"
			};
			imageEditorDefinition.Parameters["Language"] = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			imageEditorDefinition.Parameters["LocalizationPath"] = "~/DesktopModules/Admin/RadEditorProvider/App_LocalResources/";

			return imageEditorDefinition;
		}

		#endregion

		#region DNN 7.1 Page Picker Control

		/// <summary>
		/// Append DnnPageDropDownList to pnlDnnPageDropDownList with Reflection
		/// </summary>
		private void InitDnnPageDropDownList(int portalId)
		{
			if (IsPostBack)
				return;

			ObjectHandle dnnPageDropDownListHandle;
			try
			{
				dnnPageDropDownListHandle = Activator.CreateInstance("DotNetNuke.Web", "DotNetNuke.Web.UI.WebControls.DnnPageDropDownList");
			}
			catch (TypeLoadException) { return; }

			_dnnPageDropDownList = (Panel)dnnPageDropDownListHandle.Unwrap();
			_dnnPageDropDownList.ID = "PagePicker";
			_dnnPageDropDownList.Width = Unit.Pixel(498);

			var undefinedItem = new ListItem(Localization.GetString("None_Specified"), String.Empty);

			// Set Control Properties
			var dnnPageDropDownListType = _dnnPageDropDownList.GetType();
			dnnPageDropDownListType.GetProperty("UndefinedItem").SetValue(_dnnPageDropDownList, undefinedItem, null);
			dnnPageDropDownListType.GetProperty("PortalId").SetValue(_dnnPageDropDownList, portalId, null);

            // "IncludeDisabledTabs" is only available on newer DNN 7 (~7.2)
            if (dnnPageDropDownListType.GetProperty("IncludeDisabledTabs") != null)
                dnnPageDropDownListType.GetProperty("IncludeDisabledTabs").SetValue(_dnnPageDropDownList, true, null);

            // "IncludeActiveTab" is only available on newer DNN 7 (~7.2.1+)
            if (dnnPageDropDownListType.GetProperty("IncludeActiveTab") != null)
                dnnPageDropDownListType.GetProperty("IncludeActiveTab").SetValue(_dnnPageDropDownList, true, null);

			var onClientSelectionChanged = (List<string>)dnnPageDropDownListType.GetProperty("OnClientSelectionChanged").GetValue(_dnnPageDropDownList, null);
			onClientSelectionChanged.Add("ToSexyContent.ItemForm.Hyperlink._pagePicker.dnn71PickerSelectionChanged");

			pnlDnnPageDropDownList.Controls.Add(_dnnPageDropDownList);
		}

		/// <summary>
		/// Load DnnPageDropDownList, set SelectedPage (if any)
		/// </summary>
		private void LoadDnnPageDropDownList(int portalId)
		{
			if (IsPostBack || _dnnPageDropDownList == null)
				return;

		    if (FieldValueEditString == null)
		        return;

			// if value matches "Page:[0-9]+", set SelectedPage-Property
			var tagMatch = Regex.Match(FieldValueEditString, @"Page:\s?([0-9]+)");
			if (tagMatch.Success)
			{
				var selectedTabId = int.Parse(tagMatch.Groups[1].Value);
				var tabInfo = new TabController().GetTab(selectedTabId, portalId, true);

				var dnnPageDropDownListType = _dnnPageDropDownList.GetType();
				dnnPageDropDownListType.GetProperty("SelectedPage").SetValue(_dnnPageDropDownList, tabInfo, null);
			}
		}

		#endregion

		protected enum DialogTypeEnum
		{
			None,
			ImageManager,
			FileManager,
			PagePicker
		}
	}
}
