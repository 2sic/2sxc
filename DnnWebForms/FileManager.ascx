<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileManager.ascx.cs" Inherits="ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms.FileManager" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<telerik:DialogOpener runat="server" ID="DialogOpener1" />

<style type="text/css">
	body { background: transparent!important; }
	.TelerikModalOverlay { background: transparent!important; }
</style>

<script type="text/javascript">
	function OnSelectedCallback(sender, args) {

		if (!args) {
			window.bridge.valueChanged(null);
			return;
		}

		var path = args.value[0] == null ? args.value.getAttribute("src", 2) : args.value[0].getAttribute("src", 2);

		if (!path)
			path = args.value.getAttribute("href", 2);

		var url = path.indexOf("%") !== -1 ? decodeURIComponent(path) : path;
		window.bridge.valueChanged(url, (window.bridge.dialogType === 'imagemanager') ? "image" : "file");
	}

	// Call this function from outside to register the actual bridge
	window.connectBridge = function (bridge) {
		window.bridge = bridge;

		$(document).ready(function() {
			var dialogOpener = $find('<%= DialogOpener1.ClientID %>');

			switch (bridge.dialogType) {
				case 'imagemanager':
					dialogOpener.open("ImageManager", { CssClasses: [] });
					break;
				case 'documentmanager':
					var args = new Telerik.Web.UI.EditorCommandEventArgs("DocumentManager", null, document.createElement("a"));
					args.CssClasses = [];
					dialogOpener.open('DocumentManager', args);
			}

		    $('.TelerikModalOverlay').click(function() {
		        window.bridge.valueChanged(null);
		    });
		});

	};

</script>