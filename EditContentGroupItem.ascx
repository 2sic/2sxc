<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditContentGroupItem.ascx.cs" Inherits="ToSic.SexyContent.EditContentGroupItem" %>

<h2 id="hSectionHead" class="dnnFormSectionHead" ClientIDMode="Static" runat="server"><a href="#"><asp:Label runat="server" ID="lblNewOrEditItemHeading"></asp:Label></a></h2>
<fieldset>
    <%--<asp:Hyperlink CssClass="hlkChangeContent" runat="server" ID="hlkChangeContent" ResourceKey="hlkChangeContent" Visible="False"></asp:Hyperlink>--%>
    <asp:LinkButton CssClass="btnReference" ID="btnReference" runat="server" ResourceKey="btnReference" CausesValidation="False" onclick="btnReference_Click" Visible="False"></asp:LinkButton>
    <asp:HiddenField runat="server" ID="hfReferenceChanged" Value="false" />
    <asp:PlaceHolder runat="server" ID="phNewOrEditItem"></asp:PlaceHolder>
    <asp:Panel runat="server" ID="pnlReferenced" Visible="False">
        <div class="dnnFormMessage dnnFormInfo">
            <asp:Label runat="server" resourcekey="lblReferenced"></asp:Label>
            <asp:LinkButton CssClass="btnClearReference dnnSecondaryAction" ID="btnClearReference" runat="server" resourcekey="btnClearReference" CausesValidation="False" onclick="btnClearReference_Click"></asp:LinkButton>
        </div>
    </asp:Panel>
</fieldset>