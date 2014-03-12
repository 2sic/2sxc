<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataImport.ascx.cs" Inherits="ToSic.SexyContent.Administration.DataImport" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register tagPrefix="sexy" tagName="ContentTypeAndDemoSelector" src="ContentTypeAndDemoSelector.ascx" %>

<%-- Setup Panel --%>
<asp:Panel runat="server" ID="pnlSetup">
    <div class="dnnForm dnnClear">  
        <h2><asp:Label runat="server" ResourceKey="lblSetupTitle"></asp:Label></h2>        
        <p><asp:Label runat="server" ID="lblIntro" ResourceKey="lblIntro"></asp:Label></p>

         <div class="dnnFormItem">
            <asp:HiddenField runat="server" ID="AppId" />
        </div>
    
        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblFileUpload" ControlName="fuFileUpload"></dnn:Label>
            <asp:FileUpload runat="server" ID="fuFileUpload" />
        </div>

        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblContentType" ControlName="ddlContentType"></dnn:Label>
            <asp:DropDownList runat="server" ID="ddlContentType" 
                DataValueField="AttributeSetID" 
                DataTextField="Name" 
                AutoPostBack="true"></asp:DropDownList>
        </div>

        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblResourceReference" ControlName="rblResourceReference"></dnn:Label>
            <asp:RadioButtonList runat="server" ID="rblResourceReference" class="dnnFormRadioButtons" 
                DataValueField="Value"
                DataTextField="Name"
                AutoPostBack="true">
            </asp:RadioButtonList>
        </div>

        <div class="dnnFormItem" runat="server" Enabled="false">
            <dnn:Label runat="server" ID="lblEntityClear" ControlName="rblEntityClear"></dnn:Label>
            <asp:RadioButtonList runat="server" ID="rblEntityClear" class="dnnFormRadioButtons" 
                DataValueField="Value"
                DataTextField="Name"
                AutoPostBack="true">
            </asp:RadioButtonList>
        </div>

        <div class="dnnFormItem" >
            <dnn:Label runat="server"></dnn:Label>
            <asp:Label runat="server" ForeColor="Red" ID="lblWarningBackup" ResourceKey="lblWarningBackup"></asp:Label>
        </div>

        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton runat="server" ID="btnContinue" class="dnnPrimaryAction" ResourceKey="btnTestData" OnClick="OnTestDataClick"></asp:LinkButton></li>
            <li><asp:HyperLink runat="server" ID="btnCancel" class="dnnSecondaryAction" ResourceKey="btnCancel" NavigateUrl="/"></asp:HyperLink></li>
        </ul>
    </div>
</asp:Panel>

<%-- Details Panel --%>
<asp:Panel runat="server" ID="pnlDetail" Visible="false">
    <div class="dnnForm dnnClear">  
        <h2><asp:Label runat="server" ResourceKey="lblDetailTitle"></asp:Label></h2> 
        <p><asp:Label runat="server" ID="lblDetailInfo"></asp:Label></p>

        <p><asp:Label runat="server" ResourceKey="lblDetailFileTitle"></asp:Label></p>
        <ul>
            <li><asp:Label runat="server" ID="lblDetailElementCount"></asp:Label></li>
            <li><asp:Label runat="server" ID="lblDetailLanguageCount"></asp:Label></li>
            <li><asp:Label runat="server" ID="lblDetailAttributes"></asp:Label></li>
        </ul>

        <p><asp:Label runat="server" ResourceKey="lblDetailEntitiesTitle"></asp:Label></p>
        <ul>
            <li><asp:Label runat="server" ID="lblDetailEntitiesCreate"></asp:Label></li>
            <li><asp:Label runat="server" ID="lblDetailEntitiesUpdate"></asp:Label></li>
            <li><asp:Label runat="server" ID="lblDetailDetailsDelete"></asp:Label></li>
            <li><asp:Label runat="server" ID="lblDetailAttributeIgnore"></asp:Label></li>
        </ul>
        
        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton runat="server" class="dnnPrimaryAction" ResourceKey="btnImportData" OnClick="OnImportDataClick"></asp:LinkButton></li>
            <li><asp:HyperLink runat="server" class="dnnSecondaryAction" ResourceKey="btnClose" NavigateUrl="/"></asp:HyperLink></li>
        </ul>
    </div>

</asp:Panel>

<%-- Error Panel --%>
<asp:Panel runat="server" ID="pnlError" Visible="false">
    <div class="dnnForm dnnClear">  
        <h2><asp:Label runat="server" ResourceKey="lblErrorTitle"></asp:Label></h2>  
        <p><asp:Label runat="server" ID="lblErrorInfo"></asp:Label></p>

        <p><asp:Label runat="server" ResourceKey="lblErrorProtocolTitle"></asp:Label></p>
        <ul runat="server" ID="ulErrorProtocol"></ul>

        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton runat="server" class="dnnPrimaryAction" ResourceKey="btnBack" OnClick="OnBackClick"></asp:LinkButton></li>
        </ul>
    </div>
</asp:Panel>

<%-- Done Panel --%>
<asp:Panel runat="server" ID="pnlDone" Visible="false">
    <div class="dnnForm dnnClear">  
        <h2><asp:Label runat="server" ResourceKey="lblDoneTitle"></asp:Label></h2>  
        <p><asp:Label runat="server" ID="lblDoneInfo"></asp:Label></p>

        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton runat="server" class="dnnPrimaryAction" ResourceKey="btnClose" NavigateUrl="/"></asp:LinkButton></li>
        </ul>
    </div>
</asp:Panel>
