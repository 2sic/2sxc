<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataExport.ascx.cs" Inherits="ToSic.SexyContent.Administration.DataExport" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register tagPrefix="sexy" tagName="ContentTypeAndDemoSelector" src="ContentTypeAndDemoSelector.ascx" %>

<div class="dnnForm dnnClear">
    <asp:Label runat="server" ID="lblIntro" class="dnnFormMessage dnnFormInfo" ResourceKey="lblIntro"></asp:Label>
    <asp:HiddenField runat="server" ID="AppId" />
    
    <div class="dnnFormItem">
        <dnn:Label runat="server" ID="lblContentType" ControlName="ddlContentType"></dnn:Label>
        <asp:DropDownList runat="server" ID="ddlContentType" 
            DataValueField="AttributeSetID" 
            DataTextField="Name" 
            AutoPostBack="true"></asp:DropDownList>
    </div>

    <div class="dnnFormItem">
        <dnn:Label runat="server" ID="lblLanguage" ControlName="ddlLanguage"></dnn:Label>
        <asp:DropDownList runat="server" ID="ddlLanguage" 
            DataValueField="DimensionID" 
            DataTextField="Name" 
            AutoPostBack="true"
            AppendDataBoundItems="true">
            <Items>
                <asp:ListItem Value="-1" ResourceKey="lblDropDownListAll" />
            </Items>
        </asp:DropDownList>
    </div>

    <div class="dnnFormItem">
        <dnn:Label runat="server" ID="lblLanguageMissing" ControlName="rblLanguageMissing"></dnn:Label>
        <asp:RadioButtonList runat="server" ID="rblLanguageMissing" class="dnnFormRadioButtons">
            <asp:ListItem Value="ignore" ResourceKey="rbLanguageMissingIgnore" Selected="True"></asp:ListItem>
            <asp:ListItem Value="create" ResourceKey="rbLanguageMissingCreate"></asp:ListItem>
        </asp:RadioButtonList>
    </div>

     <div class="dnnFormItem">
        <dnn:Label runat="server" ID="lblLanguageReference" ControlName="rblLanguageReference"></dnn:Label>
        <asp:RadioButtonList runat="server" ID="rblLanguageReference" class="dnnFormRadioButtons">
            <asp:ListItem Value="resolve" ResourceKey="rbLanguageReferenceResolve" Selected="True"></asp:ListItem>
            <asp:ListItem Value="link" ResourceKey="rbLanguageReferenceLink"></asp:ListItem>
        </asp:RadioButtonList>
    </div>

    <div class="dnnFormItem">
        <dnn:Label runat="server" ID="lblResourceReference" ControlName="rblResourceReference"></dnn:Label>
        <asp:RadioButtonList runat="server" ID="rblResourceReference" class="dnnFormRadioButtons">
            <asp:ListItem Value="resolve" ResourceKey="rbResourceReferenceResolve" Selected="True"></asp:ListItem>
            <asp:ListItem Value="link" ResourceKey="rbResourceReferenceLink"></asp:ListItem>
        </asp:RadioButtonList>
    </div>

    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton runat="server" ID="btnExportData" class="dnnPrimaryAction" ResourceKey="btnExportData" OnClick="OnExportDataClick"></asp:LinkButton></li>
        <li><asp:HyperLink runat="server" ID="btnExportBlank" class="dnnSecondaryAction" ResourceKey="btnExportBlank" OnClick="OnExportEmptyClick"></asp:HyperLink></li>
        <li><asp:HyperLink runat="server" ID="btnCancel" class="dnnSecondaryAction" ResourceKey="btnCancel" NavigateUrl="/"></asp:HyperLink></li>
    </ul>
</div>