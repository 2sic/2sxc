<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemHistory.ascx.cs" Inherits="ToSic.Eav.ManagementUI.ItemHistory" %>
<h1>History of Entity <asp:Literal runat="server" Text='<%# EntityId %>' ID="litEntityId" /></h1>
<asp:Label runat="server" ID="lblHasDraft" Visible='<%# DraftRepositoryId.HasValue %>'>Note: This Entity has a draft with ReposotiryId <%# DraftRepositoryId %></asp:Label>
<asp:GridView runat="server" ID="grdItemHistory" AutoGenerateColumns="False" DataSourceID="dsrcEntityVersions" OnRowDataBound="grdItemHistory_RowDataBound" AllowSorting="True">
	<Columns>
		<asp:BoundField DataField="VersionNumber" HeaderText="Version" SortExpression="VersionNumber" />
		<asp:BoundField DataField="ChangeId" HeaderText="ChangeId" SortExpression="ChangeId" />
		<asp:BoundField DataField="Timestamp" HeaderText="Date" SortExpression="Timestamp" />
		<asp:BoundField DataField="User" HeaderText="Who" SortExpression="User" />
		<asp:TemplateField>
			<ItemTemplate>
				<asp:HyperLink runat="server" Text="View" ID="hlkChanges" />
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
	<EmptyDataTemplate>No History available</EmptyDataTemplate>
</asp:GridView>
<asp:ObjectDataSource ID="dsrcEntityVersions" runat="server" SelectMethod="GetEntityVersions" TypeName="ToSic.Eav.EavContext" OnObjectCreating="dsrcEntityVersions_ObjectCreating" OnSelecting="dsrcEntityVersions_Selecting">
	<SelectParameters>
		<asp:Parameter Name="entityId" Type="Int32" />
	</SelectParameters>
</asp:ObjectDataSource>
<asp:Panel runat="server" ID="pnlActions">
	<asp:Hyperlink CssClass="eav-cancel" ID="hlkBack" runat="server" Text="Back" />
</asp:Panel>
