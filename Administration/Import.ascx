<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Import.ascx.cs" Inherits="ToSic.SexyContent.Import" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>
<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>


<asp:Panel runat="server" class="dnnForm dnnSexyContentImport dnnClear" id="pnlUpload">
    <%--<asp:Panel runat="server" ID="pnlGettingStartedTemplates">
        <h2 class="dnnFormSectionHead" runat="server"><asp:Label runat="server" ResourceKey="lblImportDefaultPackageHeading"></asp:Label></h2>
        <%= LocalizeString("GettingStartedPackage.Text") %><br/><br/>
        <asp:LinkButton ID="btnInstallGettingStarted" runat="server" CssClass="dnnPrimaryAction" ResourceKey="btnInstallGettingStarted" OnClick="btnInstallGettingStarted_Click"></asp:LinkButton>
        <br/><br/><br/>
    </asp:Panel>--%>
    <h2 class="dnnFormSectionHead" runat="server"><asp:Label runat="server" ID="lblImportHeading" ResourceKey="lblImportHeading"></asp:Label></h2>
    <div class="dnnFormMessage dnnFormInfo">
        <asp:Label runat="server" ID="lblImportInfo" ResourceKey="lblImportInfo"></asp:Label>
    </div>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSelectImportFile" runat="server" ControlName="fileUpload" Suffix=":"></dnn:Label>
            <asp:FileUpload ID="fileUpload" runat="server" />
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton ID="btnUpload" runat="server" CssClass="dnnPrimaryAction" ResourceKey="btnUpload" OnClick="btnUpload_Click"></asp:LinkButton></li>
        <li><asp:Hyperlink ID="hlkExport" runat="server" CssClass="dnnSecondaryAction" ResourceKey="btnExport"></asp:Hyperlink></li>
    </ul>
</asp:Panel>

<asp:Panel runat="server" class="dnnForm dnnSexyContentExport dnnClear" id="pnlSummary" Visible="false">
    <h2 class="dnnFormSectionHead" runat="server"><asp:Label runat="server" ID="lblSummaryHeading" ResourceKey="lblSummaryHeading"></asp:Label></h2>
    <div class="dnnFormMessage dnnFormInfo">
        <asp:Label runat="server" ID="lblImportDoneInfo" ResourceKey="lblImportDoneInfo"></asp:Label>
        (<a href="javascript:CollapseSuccessMessages();">Toggle Success Messages</a>)
    </div>
    <asp:ListView runat="server" ID="lstvSummary" OnItemDataBound="lstvSummary_ItemDataBound">
        <LayoutTemplate>
            <div id="itemPlaceholder" runat="server"></div>
        </LayoutTemplate>
        <ItemTemplate>
            <asp:Panel id="pnlMessage" CssClass="dnnFormMessage" runat="server">
                <%# Eval("Message") %>
            </asp:Panel>
        </ItemTemplate>
    </asp:ListView>
    <ul class="dnnActions dnnClear">
        <li><asp:HyperLink ID="hlkClose" runat="server" CssClass="dnnPrimaryAction" ResourceKey="hlkClose"></asp:HyperLink></li>
    </ul>
</asp:Panel>

<script type="text/javascript">
    function CollapseSuccessMessages() {
        $('.dnnFormSuccess').slideToggle();
    }
</script>