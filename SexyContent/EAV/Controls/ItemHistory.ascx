<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemHistory.ascx.cs" Inherits="ToSic.Eav.ManagementUI.ItemHistory" %>
<asp:GridView runat="server" ID="grdItemHistory" AutoGenerateColumns="False" DataSourceID="dsrcEntityVersions" OnRowDataBound="grdItemHistory_RowDataBound">
	<Columns>
		<asp:BoundField DataField="VersionNumber" HeaderText="Version" />
		<asp:BoundField DataField="ChangeId" HeaderText="ChangeId" />
		<asp:BoundField DataField="Timestamp" HeaderText="Date" />
		<asp:BoundField DataField="User" HeaderText="Who" />
		<asp:TemplateField>
			<ItemTemplate>
				<asp:HyperLink runat="server" Text="Changes" ID="hlkChanges" />
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</asp:GridView>
<asp:ObjectDataSource ID="dsrcEntityVersions" runat="server" SelectMethod="GetEntityVersions" TypeName="ToSic.Eav.EavContext" OnObjectCreating="dsrcEntityVersions_ObjectCreating" OnSelecting="dsrcEntityVersions_Selecting">
	<SelectParameters>
		<asp:Parameter Name="entityId" Type="Int32" />
	</SelectParameters>
</asp:ObjectDataSource>
