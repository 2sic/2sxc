<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagePicker.ascx.cs" Inherits="ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms.PagePicker" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<dnn:DnnPageDropDownList ID="ctlPagePicker" CssClass="pagePicker" runat="server" Width="498px" IncludeDisabledTabs="true" IncludeActiveTab="true" IncludeAllTabTypes="true"  />

<script type="text/javascript">
	$(window).load(function() {
		var dnnPageDropDownList = $(".pagePicker");
		var objDnnPageDropDownList = dnn[dnnPageDropDownList.attr("id")];
		if (objDnnPageDropDownList._openItemList != undefined)	// works in DNN 7.2+
			objDnnPageDropDownList._openItemList(); // expand the treeView
		dnnPageDropDownList.find(".dt-container").width(498); // set treeView width (hight can't be set)
	});

	function SelectionChanged(selectedItem) {
		var selectedTabId = selectedItem.key;
		var url = "";
		if (selectedTabId != "")
			url = "Page:" + selectedTabId;

		if (window.fieldCallback) {
			window.fieldCallback({ url: url });
		}
	}
</script>