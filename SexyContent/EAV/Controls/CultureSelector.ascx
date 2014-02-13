<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CultureSelector.ascx.cs"
	Inherits="ToSic.Eav.ManagementUI.CultureSelector" %>
<asp:ObjectDataSource ID="dsrcCultureDimension" runat="server" SelectMethod="GetDimensionChildren"
	TypeName="ToSic.Eav.EavContext" OnObjectCreating="dsrcCultureDimension_ObjectCreating">
	<SelectParameters>
		<asp:Parameter Name="systemKey" Type="String" DefaultValue="Culture" />
	</SelectParameters>
</asp:ObjectDataSource>
<asp:DropDownList ID="ddlCultureDimension" runat="server" AppendDataBoundItems="True"
	AutoPostBack="True" DataSourceID="dsrcCultureDimension" DataTextField="Name"
	DataValueField="DimensionID" OnDataBound="ddlCultureDimension_DataBound" 
	OnSelectedIndexChanged="ddlCultureDimension_SelectedIndexChanged">
</asp:DropDownList>
