<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataImport.ascx.cs" Inherits="ToSic.SexyContent.Administration.DataImport" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>
<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

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
            <asp:FileUpload runat="server" ID="fuFileUpload" /><br />
            <asp:Label runat="server" ID="lblFileUploadError" ResourceKey="lblFileUploadError" Visible="false" ForeColor="Red"></asp:Label>
        </div>

        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblContentType" ControlName="ddlContentType"></dnn:Label>
            <asp:DropDownList runat="server" ID="ddlContentType" 
                DataValueField="AttributeSetID" 
                DataTextField="Name"></asp:DropDownList>
        </div>

        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblLanguage" ControlName="lblLanguageInfo"></dnn:Label>
            <asp:Label runat="server" ID="lblLanguageInfo" ResourceKey="lblLanguageInfo" style="display:inline-block;padding-top:3px"></asp:Label>
        </div>

        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblLanguageReference" ControlName="lblLanguageReferenceInfo"></dnn:Label>
            <asp:Label runat="server" ID="lblLanguageReferenceInfo" ResourceKey="lblLanguageReferenceInfo" style="display:inline-block;padding-top:3px"></asp:Label>
        </div>

        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblResourceReference" ControlName="rblResourceReference"></dnn:Label>
            <asp:RadioButtonList runat="server" ID="rblResourceReference" class="dnnFormRadioButtons" 
                DataValueField="Value"
                DataTextField="Name">
            </asp:RadioButtonList>
        </div>

        <div class="dnnFormItem" runat="server">
            <dnn:Label runat="server" ID="lblEntityClear" ControlName="rblEntityClear"></dnn:Label>
            <asp:RadioButtonList runat="server" ID="rblEntityClear" class="dnnFormRadioButtons" 
                DataValueField="Value"
                DataTextField="Name">
            </asp:RadioButtonList>
        </div>

        <div class="dnnFormItem" >
            <dnn:Label runat="server"></dnn:Label>
            <asp:Label runat="server" ForeColor="Red" ID="lblWarningBackup" ResourceKey="lblWarningBackup"></asp:Label>
        </div>

        <div class="dnnFormItem" >
            <dnn:Label runat="server"></dnn:Label>
            <asp:Label runat="server" ForeColor="DarkOrange" ID="lblInformationLimitations" ResourceKey="lblInformationLimitations"></asp:Label>
        </div>

        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton runat="server" ID="btnContinue" class="dnnPrimaryAction" ResourceKey="btnTestData" OnClick="OnTestDataClick"></asp:LinkButton></li>
            <li><asp:HyperLink runat="server" class="dnnSecondaryAction" ResourceKey="btnClose" NavigateUrl="/"></asp:HyperLink></li>
            <li><asp:LinkButton runat="server" ID="btnContinueDetailed" ForeColor="White" ResourceKey="btnTestDataDetailed" OnClick="OnTestDataDetailedClick"></asp:LinkButton></li>
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

        <p>
            <asp:Label runat="server" ID="lblDetailDebugOutput"></asp:Label>
        </p>
        
        <p>
            <asp:Label runat="server" ForeColor="DarkOrange" ResourceKey="lblInformationImportTime"></asp:Label>
        </p>

        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton runat="server" class="dnnPrimaryAction" ResourceKey="btnImportData" OnClick="OnImportDataClick"></asp:LinkButton></li>
            <li><asp:LinkButton runat="server" class="dnnSecondaryAction" ResourceKey="btnBack" OnClick="OnBackClick"></asp:LinkButton></li>
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
            <li><asp:HyperLink runat="server" class="dnnSecondaryAction" ResourceKey="btnClose" NavigateUrl="/"></asp:HyperLink></li>
        </ul>
    </div>
</asp:Panel>

<%-- Done Panel --%>
<asp:Panel runat="server" ID="pnlDone" Visible="false">
    <div class="dnnForm dnnClear">  
        <h2><asp:Label runat="server" ResourceKey="lblDoneTitle"></asp:Label></h2>  
        <p><asp:Label runat="server" ID="lblDoneInfo"></asp:Label></p>
        <p><asp:Label runat="server" ID="lblDoneResult"></asp:Label></p>

        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton runat="server" class="dnnPrimaryAction" ResourceKey="btnBack" OnClick="OnBackClick"></asp:LinkButton></li>
            <li><asp:HyperLink runat="server" class="dnnSecondaryAction" ResourceKey="btnClose" NavigateUrl="/"></asp:HyperLink></li>
        </ul>
    </div>
</asp:Panel>
