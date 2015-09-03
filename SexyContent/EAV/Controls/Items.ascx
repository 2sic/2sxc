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
	OnContextCreating="dsrcAttributeSet_ContextCreating">
	<WhereParameters>
		<asp:Parameter Name="AttributeSetId" Type="Int32" />
	</WhereParameters>
</asp:EntityDataSource>
<asp:Label runat="server" ID="lblNotifications" />
<asp:GridView ID="grdItems" runat="server" DataSourceID="dsrcItems" AllowSorting="True" 
	OnRowDataBound="grdItems_RowDataBound" DataKeyNames="RepositoryId" OnDataBound="grdItems_DataBound" AllowPaging="True" PageSize="100" OnDataBinding="grdItems_DataBinding">
	<Columns>
		<asp:HyperLinkField Text="Edit" />
		<asp:ButtonField CommandName="Delete" Text="Delete" />
	</Columns>
	<EmptyDataTemplate>
		No Records
	</EmptyDataTemplate>
</asp:GridView>
<asp:ObjectDataSource ID="dsrcItems" runat="server" SelectMethod="GetItemsTable"
	TypeName="ToSic.Eav.AscxHelpers.ListForSomeAscx" 
	OnObjectCreating="dsrcItems_ObjectCreating" OnSelecting="dsrcItems_Selecting" OnDeleting="dsrcItems_Deleting" DeleteMethod="DeleteEntity" OnDeleted="dsrcItems_Deleted">
	<SelectParameters>
		<asp:Parameter Name="attributeSetId" Type="Int32" />
		<asp:Parameter Name="dimensionIds" Type="Object" />
		<asp:Parameter Name="maxValueLength" Type="Int32" DefaultValue="200" />
	</SelectParameters>
	<DeleteParameters>
		<asp:Parameter Name="RepositoryId" Type="Int32" />
	</DeleteParameters>
</asp:ObjectDataSource>