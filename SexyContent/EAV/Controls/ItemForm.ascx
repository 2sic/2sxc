<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemForm.ascx.cs" Inherits="ToSic.Eav.ManagementUI.ItemForm" %>
<script type="text/javascript">
    var EavEntityModels;
    if (EavEntityModels == undefined)
        EavEntityModels = new Object();
    EavEntityModels[<asp:Literal runat='server' id='litJsonEntityId' />] = <asp:Literal runat='server' id='litJsonEntityModel' />;
    var EavDimensionsModel = <asp:Literal runat='server' id='litJsonDimensionsModel' />
</script>
<asp:Panel ID="pnlNavigateBack" runat="server" Visible='<%# IsDialog %>'>
	<asp:HyperLink ID="hlnkNavigateBack" NavigateUrl='<%# ReturnUrl %>' runat="server" Text="Back" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlEditForm" CssClass="eav-form" ng-app="2sic-EAV">
	<asp:PlaceHolder runat="server" ID="phFields" />
	<div class='eav-field-system eav-field-published dnnFormItem'>
		<asp:Label ID="FieldLabel" runat="server" Text="Published" />
		<div class="eav-field-control">
			<asp:RadioButtonList runat="server" ID="rblPublished" CssClass="" EnableViewState="True" RepeatDirection="Horizontal">
				<asp:ListItem Value="Published" Text="Yes" />
				<asp:ListItem Value="Draft" Text="No" />
			</asp:RadioButtonList>
		</div>
	</div>
	<br />
</asp:Panel>
<asp:Panel runat="server" ID="pnlEditDefaultFirstEN" CssClass="eav-message-defaultfirst dnnFormMessage dnnFormInfo" Visible="False">
</asp:Panel>
<%--<asp:Panel runat="server" ID="pnlEditDefaultFirstDE" CssClass="eav-message-defaultfirst dnnFormMessage dnnFormInfo" Visible="False">
    Bitte zuerst in der Standardsprache erfassen.
</asp:Panel>--%>
<asp:Panel runat="server" ID="pnlActions" Visible='<%# !HideNavigationButtons %>'>
    <asp:LinkButton CssClass="eav-save" ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
	<asp:LinkButton CssClass="eav-cancel" ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Click" />
	<asp:Hyperlink CssClass="eav-history" ID="hlkShowHistory" runat="server" Text="Show History" NavigateUrl='<%# GetHistoryUrl() %>' />
</asp:Panel>