<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.EditTemplate" Codebehind="EditTemplate.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register tagPrefix="Sexy" tagName="ContentTypeAndDemoSelector" src="ContentTypeAndDemoSelector.ascx" %>

<div class="dnnForm scEditTemplate dnnClear">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelSexyContentNewEditTemplate">
        <a href="#"><asp:Label runat="server" ID="lblEditTemplateHeading" ResourceKey="lblEditTemplateHeading"></asp:Label></a></h2>
    <fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblTemplateName" runat="server" ControlName="txtTemplateName" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtTemplateName" runat="server" CssClass="dnnFormRequired"></asp:TextBox>
        <asp:RequiredFieldValidator ID="valTemplateName" runat="server" ControlToValidate="txtTemplateName" CssClass="dnnFormError" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblTemplateType" runat="server" ControlName="ddlTemplateTypes" Suffix=":" />
        <asp:DropDownList runat="server" ID="ddlTemplateTypes" AutoPostBack="true" OnSelectedIndexChanged="ddlTemplateTypes_SelectedIndexChanged">
            <asp:ListItem ResourceKey="TokenListItem" Value="Token"></asp:ListItem>
            <asp:ListItem ResourceKey="CSharpRazorListItem" Value="C# Razor"></asp:ListItem>
            <asp:ListItem ResourceKey="VBRazorListItem" Value="VB Razor"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblTemplateLocation" runat="server" ControlName="ddlTemplateLocations" Suffix=":" />
	    <asp:DropDownList runat="server" ID="ddlTemplateLocations" AutoPostBack="true" OnSelectedIndexChanged="ddlTemplateLocations_SelectedIndexChanged" />
    </div>
    <asp:Panel runat="server" ID="pnlSelectTemplateFile" CssClass="dnnFormItem">
        <dnn:Label ID="lblTemplateFile" runat="server" ControlName="ddlTemplateFiles" Suffix=":" />
	    <asp:DropDownList runat="server" ID="ddlTemplateFiles" />
	    <asp:LinkButton runat="server" ID="btnCreateTemplateFile" ResourceKey="btnCreateTemplateFile" OnClick="btnCreateTemplateFile_Click" CausesValidation="false" />
        <asp:RequiredFieldValidator ID="valTemplateFile" runat="server" ControlToValidate="ddlTemplateFiles" CssClass="dnnFormError" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
    </asp:Panel> 
    <asp:Panel runat="server" ID="pnlCreateTemplateFile" CssClass="dnnFormItem" Visible="false">
        <dnn:Label ID="lblTemplateFileName" runat="server" ControlName="txtTemplateFileName" Suffix=":" />
	    <asp:TextBox runat="server" ID="txtTemplateFileName" />
        <asp:RequiredFieldValidator ID="valTemplateFileName" Enabled="false" runat="server" ControlToValidate="txtTemplateFileName" CssClass="dnnFormError" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
    </asp:Panel>
    <Sexy:ContentTypeAndDemoSelector runat="server" ID="ctrContentType" ItemType="Content"></Sexy:ContentTypeAndDemoSelector>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSeparateContentPresentation" runat="server" ControlName="chkSeparateContentPresentation" Suffix=":"></dnn:Label>
        <asp:CheckBox runat="server" ID="chkSeparateContentPresentation" Checked="False" AutoPostBack="True" />
    </div>
    <asp:Panel runat="server" ID="pnlSeparateContentPresentation" Visible="False">
        <Sexy:ContentTypeAndDemoSelector runat="server" ID="ctrPresentationType" ItemType="Presentation"></Sexy:ContentTypeAndDemoSelector>
    </asp:Panel>
    <div class="dnnFormItem">
        <dnn:Label ID="lblEnableList" runat="server" ControlName="chkEnableList" Suffix=":" />
        <asp:CheckBox ID="chkEnableList" AutoPostBack="True" runat="server" />
    </div>
    <asp:Panel runat="server" ID="pnlListConfiguration" Visible="False">
        <Sexy:ContentTypeAndDemoSelector runat="server" ID="ctrListContentType" ItemType="ListContent"></Sexy:ContentTypeAndDemoSelector>
        <Sexy:ContentTypeAndDemoSelector runat="server" ID="ctrListPresentationType" ItemType="ListPresentation"></Sexy:ContentTypeAndDemoSelector>
    </asp:Panel>
    <div class="dnnFormItem">
        <dnn:Label ID="lblHidden" runat="server" ControlName="chkVisible" Suffix=":" />
        <asp:CheckBox ID="chkHidden" runat="server" />
    </div>
    </fieldset>
    <h2 class="dnnFormSectionHead" id="dnnSitePanelSexyContentTemplateDataPublish">
        <a href="#"><asp:Label runat="server" ID="lblDataSourcePublishing" ResourceKey="lblDataSourcePublishing"></asp:Label></a>
    </h2>
    <fieldset>
		<asp:Panel runat="server" ID="pnlDataPipeline" CssClass="dnnFormItem">
			<dnn:Label ID="lblPipelineEntity" runat="server" ControlName="ddlDataPipeline" Suffix=":" />
			<asp:DropDownList runat="server" ID="ddlDataPipeline" DataValueField="PipelineEntityID" DataTextField="Name" AppendDataBoundItems="True">
				<asp:ListItem Value="0" ResourceKey="ddlDataPipelineDefaultItem" />
			</asp:DropDownList>
			<asp:HyperLink runat="server" ID="hlkManagePipelines" ResourceKey="hlkManagePipelines" />
		</asp:Panel>
		<asp:Panel runat="server" ID="pnlViewNameInUrl" CssClass="dnnFormItem">
			<dnn:Label ID="lblViewNameInUrl" runat="server" ControlName="txtViewNameInUrl" Suffix=":" />
			<asp:TextBox runat="server" ID="txtViewNameInUrl" />
			<asp:RegularExpressionValidator ID="valViewNameInUrl" ResourceKey="valViewNameInUrl" runat="server" ValidationExpression="[^/]+/[^/]+" ControlToValidate="txtViewNameInUrl" CssClass="dnnFormError" Display="Dynamic" EnableClientScript="true" />
		</asp:Panel>
        <div style="margin-left:280px; margin-bottom:30px;"><%= LocalizeString("lblDataSourcePublishing.LongText") %></div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblPublishSource" runat="server" ControlName="chkPublishSource" Suffix=":"></dnn:Label>
            <asp:CheckBox runat="server" ID="chkPublishSource" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblPublishStreams" runat="server" ControlName="txtPublishStreams" Suffix=":"></dnn:Label>
            <asp:TextBox runat="server" ID="txtPublishStreams" Text="Default,ListContent"></asp:TextBox>
        </div>
    </fieldset>
    
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton runat="server" ID="btnUpdate" ResourceKey="btnUpdate" OnClick="btnUpdate_Click" CssClass="dnnPrimaryAction"></asp:LinkButton></li>
        <li><asp:HyperLink runat="server" ID="hlkCancel" ResourceKey="hlkCancel" CssClass="dnnSecondaryAction"></asp:HyperLink></li>
    </ul>
</div>

<script type="text/javascript">
    $(document).ready(function() {
        var setupModule = function() {
            jQuery('.scEditTemplate').dnnPanels();
        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
            // note that this will fire when _any_ UpdatePanel is triggered
            setupModule();
        });
    });
</script>