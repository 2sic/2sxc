using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using Telerik.Web.UI;
using Telerik.Web.UI.Editor.DialogControls;
using ToSic.SexyContent.EAV.FieldTemplates;

namespace ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms
{
	public partial class FileManager : UserControl
	{
		public int PortalId { get; set; }

		private ImageManagerDialogConfiguration _imageManagerConfiguration = new ImageManagerDialogConfiguration();
		private FileManagerDialogConfiguration _documentManagerConfiguration = new FileManagerDialogConfiguration();
		private EditorProvider _editorProvider;

		protected void Page_Load(object sender, EventArgs e)
		{
			InitDialogOpener();
		}

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
			//var homeDirectory = PortalSettings.Current.HomeDirectory;
			//string[] paths;
			//var metaDataPaths = GetMetaDataValue<string>("Paths");
			//if (!String.IsNullOrEmpty(metaDataPaths))
			//	paths = metaDataPaths.Split(',').Select(p => homeDirectory + p).ToArray();
			//else
			var paths = _imageManagerConfiguration.ViewPaths;

			#endregion

			DialogOpener1.DialogDefinitions.Add("ImageManager", GetImageManagerDefinition(paths));
			DialogOpener1.DialogDefinitions.Add("ImageEditor", GetImageEditorDefinition(paths));
			DialogOpener1.DialogDefinitions.Add("DocumentManager", GetDocumentManagerDefinition(paths));

			DialogOpener1.HandlerUrl = "~/DesktopModules/Admin/RadEditorProvider/DialogHandler.aspx?portalid=" + PortalSettings.Current.PortalId + "&tabid=" + PortalSettings.Current.ActiveTab.TabID;
			
			//if (!String.IsNullOrWhiteSpace(FieldValueEditString) && FieldValueEditString.StartsWith("File:"))
			//	DialogOpener1.AdditionalQueryString = "&PreselectedItemUrl=" + HttpUtility.UrlEncode(SexyContent.SexyContent.ResolveHyperlinkValues(FieldValueEditString, PortalSettings.Current));

			DialogOpener1.EnableEmbeddedSkins = _editorProvider._editor.EnableEmbeddedSkins;
			DialogOpener1.Skin = _editorProvider._editor.Skin;
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

			//if (!String.IsNullOrEmpty(GetMetaDataValue<string>("FileFilter")))
			//	imageManagerParameters.SearchPatterns = GetMetaDataValue<string>("FileFilter").Split(',');

			var imageManagerDefinition = new DialogDefinition(typeof(ImageManagerDialog), imageManagerParameters)
			{
				ClientCallbackFunction = "OnSelectedCallback",
				Width = Unit.Pixel(694),
				Height = Unit.Pixel(440),
				Title = "Image Manager"
			};

			imageManagerDefinition.Parameters["Language"] = Thread.CurrentThread.CurrentCulture.Name;
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
			
			//if (!String.IsNullOrEmpty(GetMetaDataValue<string>("FileFilter")))
			//	documentManagerParameters.SearchPatterns = GetMetaDataValue<string>("FileFilter").Split(',');

			var documentManagerDefinition = new DialogDefinition(typeof(DocumentManagerDialog), documentManagerParameters)
			{
				ClientCallbackFunction = "OnSelectedCallback",
				Width = Unit.Pixel(694),
				Height = Unit.Pixel(440),
				Title = "Document Manager"
			};
			documentManagerDefinition.Parameters["Language"] = Thread.CurrentThread.CurrentCulture.Name;
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

			//if (!String.IsNullOrEmpty(GetMetaDataValue<string>("FileFilter")))
			//	imageEditorParameters.SearchPatterns = GetMetaDataValue<string>("FileFilter").Split(',');

			var imageEditorDefinition = new DialogDefinition(typeof(ImageEditorDialog), imageEditorParameters)
			{
				Width = Unit.Pixel(832),
				Height = Unit.Pixel(500),
				Title = "Image Editor"
			};
			imageEditorDefinition.Parameters["Language"] = Thread.CurrentThread.CurrentCulture.Name;
			imageEditorDefinition.Parameters["LocalizationPath"] = "~/DesktopModules/Admin/RadEditorProvider/App_LocalResources/";

			return imageEditorDefinition;
		}
	}
}