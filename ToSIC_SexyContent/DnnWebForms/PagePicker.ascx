<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagePicker.ascx.cs" Inherits="ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms.PagePicker" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<dnn:DnnPageDropDownList ID="ctlPagePicker" CssClass="pagePicker" runat="server" Width="563px" IncludeDisabledTabs="true" IncludeActiveTab="true" IncludeAllTabTypes="true"  />

<script type="text/javascript">
	// Call this function from outside to register the actual bridge
	window.connectBridge = function (bridge) {
		window.bridge = bridge;
		$(document).ready(function () {
			initDropDown();
		});
	};

	function initDropDown() {
		var dnnPageDropDownList = $(".pagePicker");
		var objDnnPageDropDownList = dnn[dnnPageDropDownList.attr("id")];
		if (objDnnPageDropDownList._openItemList != undefined)	// works in DNN 7.2+
			objDnnPageDropDownList._openItemList(); // expand the treeView
		dnnPageDropDownList.find(".dt-container").width(563); // set treeView width (hight can't be set)
	}

	function SelectionChanged(selectedItem) {
        var result = {}
		if (selectedItem.key !== "") 
            result = {
                id: selectedItem.key,
                name: selectedItem.value
            }
	    bridge.valueChanged(result, "page");
	}
</script>

<style type="text/css">
	.dnnDropDownList .dt-container { height: 298px; }
	.dnnDropDownList .dt-content { height: 226px; }
</style>