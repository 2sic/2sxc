<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataExport.ascx.cs" Inherits="ToSic.SexyContent.Administration.DataExport" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register tagPrefix="sexy" tagName="ContentTypeAndDemoSelector" src="ContentTypeAndDemoSelector.ascx" %>

<div class="dnnForm dnnClear">
    <div class="dnnFormMessage dnnFormInfo">
        <asp:Label runat="server" ID="lblIntro" ResourceKey="lblIntro"></asp:Label>
    </div>

     <div class="dnnFormItem">
        <asp:HiddenField runat="server" ID="AppId" />
    </div>
    
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
            AutoPostBack="true"
            AppendDataBoundItems="true">
            <Items>
                <asp:ListItem Value="" ResourceKey="lblDropDownListAll" />
            </Items>
        </asp:DropDownList>
    </div>

    <div class="dnnFormItem">
        <dnn:Label runat="server" ID="lblRecordExport" ControlName="rblRecordExport"></dnn:Label>
        <asp:RadioButtonList runat="server" ID="rblRecordExport" class="dnnFormRadioButtons" 
            DataValueField="Value"
            DataTextField="Name"
            OnSelectedIndexChanged="OnRecordExportSelectedIndexChanged" 
            AutoPostBack="true">
        </asp:RadioButtonList>
    </div>

    <asp:Panel runat="server" ID="pnlExportReferenceOptions">
        <div class="dnnFormItem" style="display: none">
            <dnn:Label runat="server" ID="lblLanguageMissing" ControlName="rblLanguageMissing"></dnn:Label>
            <asp:RadioButtonList runat="server" ID="rblLanguageMissing" class="dnnFormRadioButtons" 
                DataValueField="Value"
                DataTextField="Name"
                AutoPostBack="true">
            </asp:RadioButtonList>
        </div>

         <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblLanguageReference" ControlName="rblLanguageReference"></dnn:Label>
            <asp:RadioButtonList runat="server" ID="rblLanguageReference" class="dnnFormRadioButtons" 
                DataValueField="Value"
                DataTextField="Name"
                AutoPostBack="true">
            </asp:RadioButtonList>
        </div>

        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblResourceReference" ControlName="rblResourceReference"></dnn:Label>
            <asp:RadioButtonList runat="server" ID="rblResourceReference" class="dnnFormRadioButtons" 
                DataValueField="Value"
                DataTextField="Name"
                AutoPostBack="true">
            </asp:RadioButtonList>
        </div>
    </asp:Panel>

    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton runat="server" ID="btnExportData" class="dnnPrimaryAction" ResourceKey="btnExportData" OnClick="OnExportDataClick"></asp:LinkButton></li>
        <li><asp:HyperLink runat="server" ID="btnCancel" class="dnnSecondaryAction" ResourceKey="btnCancel" NavigateUrl="/"></asp:HyperLink></li>
    </ul>
</div>