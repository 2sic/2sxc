<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Wysiwyg.ascx.cs" Inherits="ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms.Wysiwyg" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>

<dnn:texteditor Visible="false" Height="400" Width="100%" ID="Texteditor1" runat="server" EnableViewState="true" HtmlEncode="false" />

<script type="text/javascript">
	$(document).ready(function() {
		var x = $find('<%= Texteditor1.ClientID %>');
		var b = 12;
	});

	//window.setTimeout(function () {
	//	alert('inner alert');
	//	window.bridge.onChanged("new value of wysiwyg control");
	//},4000);
</script>