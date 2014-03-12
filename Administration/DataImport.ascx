<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataImport.ascx.cs" Inherits="ToSic.SexyContent.Administration.DataImport" %>
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
        <dnn:Label runat="server" ID="lblFileUpload" ControlName="FileUpload"></dnn:Label>
        <asp:FileUpload runat="server" ID="FileUpload" EnableViewState="true"/>
    </div>

    <div class="dnnFormItem">
        <dnn:Label runat="server" ID="lblContentType" ControlName="ddlContentType"></dnn:Label>
        <asp:DropDownList runat="server" ID="ddlContentType" 
            DataValueField="AttributeSetID" 
            DataTextField="Name" 
            AutoPostBack="true"></asp:DropDownList>
    </div>

    <div class="dnnFormItem">
        <dnn:Label runat="server" ID="lblEntityCreate" ControlName="rblEntityCreate"></dnn:Label>
        <asp:RadioButtonList runat="server" ID="rblEntityCreate" class="dnnFormRadioButtons" 
            DataValueField="Value"
            DataTextField="Name"
            AutoPostBack="true">
        </asp:RadioButtonList>
    </div>

    <div class="dnnFormItem" runat="server" Visible="false">
        <dnn:Label runat="server" ID="lblEntityClear" ControlName="rblEntityClear"></dnn:Label>
        <asp:RadioButtonList runat="server" ID="rblEntityClear" class="dnnFormRadioButtons" 
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

    <div class="dnnFormMessage dnnFormWarning">
        <asp:Label runat="server" ID="lblWarningBackup" ResourceKey="lblWarningBackup"></asp:Label>
    </div>

    <div class="dnnFormMessage">
        <asp:Label runat="server" ID="lblOutput" ></asp:Label>
    </div>

    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton runat="server" ID="btnContinue" class="dnnPrimaryAction" ResourceKey="btnContinue" OnClick="OnContinueClick"></asp:LinkButton></li>
        <li><asp:HyperLink runat="server" ID="btnCancel" class="dnnSecondaryAction" ResourceKey="btnCancel" NavigateUrl="/"></asp:HyperLink></li>
    </ul>
</div>