<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditEntity.ascx.cs" Inherits="ToSic.SexyContent.EditEntity" %>

<h2 id="hSectionHead" class="dnnFormSectionHead" ClientIDMode="Static" runat="server"><a href="#"><asp:Label runat="server" ID="lblNewOrEditItemHeading"></asp:Label></a></h2>
<fieldset>
    <asp:HyperLink style="float:right;" runat="server" ID="hlkHistory" Visible="False"><%=LocalizeString("HistoryLink.Text") %></asp:HyperLink>
    <asp:PlaceHolder runat="server" ID="phNewOrEditItem"></asp:PlaceHolder>
</fieldset>