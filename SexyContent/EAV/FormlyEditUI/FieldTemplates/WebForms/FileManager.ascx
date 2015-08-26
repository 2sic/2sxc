<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileManager.ascx.cs" Inherits="ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms.FileManager" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<telerik:DialogOpener runat="server" ID="DialogOpener1" />

<script type="text/javascript">
	function OnSelectedCallback(sender, args) {
		if (!args)
			return;

		var path = args.value[0] == null ? args.value.getAttribute("src", 2) : args.value[0].getAttribute("src", 2);
		var url = path.indexOf("%") != -1 ? decodeURIComponent(path) : path;

		//ToSexyContent.ItemForm.Hyperlink._fileManagerCallback(sender, args, url);
		alert(url);
	}

	$(document).ready(function() {
		$find('<%= DialogOpener1.ClientID %>').open('ImageManager', { CssClasses: [] });
	});

	//OpenDialog: function (sender, dialogOpenerId, type, attributeStaticName, portalId, portalHomeDirectory) {
	//	switch (type) {
	//		case "ImageManager":
	//			$find(dialogOpenerId).open('ImageManager', { CssClasses: [], AttributeStaticName: attributeStaticName, PortalId: portalId, PortalHomeDirectory: portalHomeDirectory });
	//			break;
	//		case "FileManager":
	//			var args = new Telerik.Web.UI.EditorCommandEventArgs("DocumentManager", null, document.createElement("a"));
	//			args.CssClasses = [];
	//			args.AttributeStaticName = attributeStaticName;
	//			args.PortalId = portalId;
	//			args.PortalHomeDirectory = portalHomeDirectory;

	//			$find(dialogOpenerId).open('DocumentManager', args);
	//			break;
</script>