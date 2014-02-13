<%@ Control Language="C#" AutoEventWireup="True" Inherits="ToSic.Eav.ManagementUI.Items" Codebehind="Items.ascx.cs" %>
<asp:Panel ID="pnlNavigateBack" runat="server" Visible='<%# !IsDialog %>'>
	<asp:HyperLink ID="hlnkNavigateBack" NavigateUrl='<%# ReturnUrl %>' runat="server" Text="Back" />
</asp:Panel>
<asp:HyperLink NavigateUrl='<%# GetNewItemUrl() %>' Text="New Item" runat="server" ID="hlnkNewItem" />
<asp:ListView ID="lstvHeading" runat="server" DataSourceID="dsrcAttributeSet">
	<ItemTemplate>
		<h1>
			<asp:Label ID="lblHeading" runat="server" Text="Items:" />
			<asp:Label ID="lblAttributeSetName" runat="server" Text='<%# Eval("Name") %>' />
		</h1>
	</ItemTemplate>
	<LayoutTemplate>
		<asp:PlaceHolder ID="itemPlaceholder" runat="server" />
	</LayoutTemplate>
	<EmptyDataTemplate>
		AttributeSet not found
	</EmptyDataTemplate>
</asp:ListView>
<asp:EntityDataSource ID="dsrcAttributeSet" runat="server" ConnectionString="name=EavContext"
	DefaultContainerName="EavContext" EnableFlattening="False" EntitySetName="AttributeSets"
	EntityTypeFilter="AttributeSet" Include="" 
	Where="it.AttributeSetId = @AttributeSetId" 
	oncontextcreating="dsrcAttributeSet_ContextCreating">
	<WhereParameters>
		<asp:Parameter Name="AttributeSetId" Type="Int32" />
	</WhereParameters>
</asp:EntityDataSource>
<asp:GridView ID="grdItems" runat="server" DataSourceID="dsrcItems" 
	AutoGenerateColumns="true" AllowSorting="True" 
	onrowdatabound="grdItems_RowDataBound">
	<EmptyDataTemplate>
		No Records
	</EmptyDataTemplate>
</asp:GridView>
<asp:ObjectDataSource ID="dsrcItems" runat="server" SelectMethod="GetItemsTable"
	TypeName="ToSic.Eav.EavContext" 
	onobjectcreating="dsrcItems_ObjectCreating" onselecting="dsrcItems_Selecting">
	<SelectParameters>
		<asp:Parameter Name="attributeSetId" Type="Int32" />
		<asp:Parameter Name="dimensionIds" Type="Object" />
		<asp:Parameter Name="source" Type="Object" ConvertEmptyStringToNull="True" DefaultValue="" />
	</SelectParameters>
</asp:ObjectDataSource>
