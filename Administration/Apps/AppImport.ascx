<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppImport.ascx.cs" Inherits="ToSic.SexyContent.AppImport" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<asp:Panel runat="server" class="dnnForm dnnClear" id="pnlUpload">
    <h2 class="dnnFormSectionHead" runat="server">Import App</h2>
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
        <li><asp:LinkButton ID="btnUpload" runat="server" CssClass="dnnPrimaryAction" Text="Upload" ResourceKey="btnUpload" OnClick="btnUpload_Click" OnClientClick="$(this).hide();"></asp:LinkButton></li>
    </ul>
</asp:Panel>

<asp:Panel runat="server" class="dnnForm dnnClear" id="pnlSummary" Visible="false">
    <h2 class="dnnFormSectionHead">Summary</h2>
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
        <li>
            <a href="<%= EditUrl("", "", "appmanagement") %>" class="dnnPrimaryAction"><%= LocalizeString("hlkClose.Text") %></a>
        </li>
    </ul>
</asp:Panel>

<script type="text/javascript">
    function CollapseSuccessMessages() {
        $('.dnnFormSuccess').slideToggle();
    }
</script>