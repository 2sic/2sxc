using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using Telerik.Web.UI;
using Telerik.Web.UI.Editor.DialogControls;
using ToSic.Eav.Implementations.ValueConverter;
using ToSic.SexyContent.EAV.FieldTemplates;
using ToSic.SexyContent.EAV.Implementation.ValueConverter;

namespace ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms
{
	public partial class FileManager : UserControl
	{
		/* Parameters provided through QueryString parameters */
		private string Paths {
			get { return Request.QueryString["Paths"] ?? ""; }
		}

		private string CurrentValue
		{
			get { return Request.QueryString["CurrentValue"]; }
		}

		private string FileFilter
		{
			get { return Request.QueryString["FileFilter"]; }
		}


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
		    try
		    {
		        _editorProvider = new EditorProvider();

		        _editorProvider.Initialize();
		        _imageManagerConfiguration = _editorProvider._editor.ImageManager;
		        _documentManagerConfiguration = _editorProvider._editor.DocumentManager;
		    }
		    catch (Exception ex)
		    {
		        throw new Exception("had trouble initializing the editor provider (telerik) - you're probably using dnn 8 and didn't install the old telerik extension.", ex);
		    }
		}

		private void InitDialogOpener()
		{
			InitEditorProvider();

			#region Get View, Update, DeletePaths
			if(Paths.Contains(".."))
				throw new Exception("Invalid Paths parameter provided.");
			var homeDirectory = PortalSettings.Current.HomeDirectory;
			string[] paths;
			if (!String.IsNullOrEmpty(Paths))
				paths = Paths.Split(',').Select(p => homeDirectory + p.Trim()).ToArray();
			else
				paths = _imageManagerConfiguration.ViewPaths;

			#endregion

			DialogOpener1.DialogDefinitions.Add("ImageManager", GetImageManagerDefinition(paths));
			DialogOpener1.DialogDefinitions.Add("ImageEditor", GetImageEditorDefinition(paths));
			DialogOpener1.DialogDefinitions.Add("DocumentManager", GetDocumentManagerDefinition(paths));

			DialogOpener1.HandlerUrl = "~/DesktopModules/Admin/RadEditorProvider/DialogHandler.aspx?portalid=" + PortalSettings.Current.PortalId + "&tabid=" + PortalSettings.Current.ActiveTab.TabID;

		    if (!String.IsNullOrWhiteSpace(CurrentValue) && CurrentValue.StartsWith("file:", StringComparison.InvariantCultureIgnoreCase))
		    {
                var conv = new SexyContentValueConverter();
		        var realPath = conv.Convert(ConversionScenario.GetFriendlyValue, "Hyperlink", CurrentValue);

		        DialogOpener1.AdditionalQueryString = "&PreselectedItemUrl=" +
		                                              HttpUtility.UrlEncode(realPath
                                                          /*SexyContent.ResolveHyperlinkValues(CurrentValue,
		                                                  PortalSettings.Current)*/);
		    }

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

			if (!String.IsNullOrEmpty(FileFilter))
				imageManagerParameters.SearchPatterns = FileFilter.Split(',').Select(v => v.Trim()).ToArray();

			var imageManagerDefinition = new DialogDefinition(typeof(ImageManagerDialog), imageManagerParameters)
			{
				ClientCallbackFunction = "OnSelectedCallback",
				Width = Unit.Pixel(832),
				Height = Unit.Pixel(500),
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

			if (!String.IsNullOrEmpty(FileFilter))
				documentManagerParameters.SearchPatterns = FileFilter.Split(',').Select(v => v.Trim()).ToArray();

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

			if (!String.IsNullOrEmpty(FileFilter))
				imageEditorParameters.SearchPatterns = FileFilter.Split(',').Select(v => v.Trim()).ToArray();

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