<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateHelpGrid.ascx.cs" Inherits="ToSic.SexyContent.TemplateHelpGrid" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>

<dnnweb:DnnGrid ID="grdFields" AutoGenerateColumns="false" runat="server" Width="500px" ShowHeader="true" CssClass="grdFields">
    <MasterTableView ShowHeader="true" DataKeyNames="StaticName">
        <Columns>
            <dnnweb:DnnGridBoundColumn DataField="StaticName" HeaderText="ColumnToken" UniqueName="StaticName" ItemStyle-Width="200px" HeaderStyle-Font-Bold="True"></dnnweb:DnnGridBoundColumn>
            <dnnweb:DnnGridBoundColumn DataField="DisplayName" HeaderText="ColumnField"></dnnweb:DnnGridBoundColumn>
        </Columns>
        <HeaderStyle />
    </MasterTableView>
</dnnweb:DnnGrid>