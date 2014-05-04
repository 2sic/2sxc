<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemVersionDetails.ascx.cs" Inherits="ToSic.Eav.ManagementUI.ItemVersionDetails" %>
<h1><asp:Literal runat="server" Text="Version {0} of {1} (Entity {2})" ID="litControlHeading"/></h1>
<h2>Result of changes</h2>
<asp:GridView runat="server" ID="grdVersionDetails" AllowSorting="True" DataSourceID="dsrcVersionDetails" OnRowDataBound="grdVersionDetails_RowDataBound" AutoGenerateColumns="False">
	<Columns>
		<asp:BoundField HeaderText="Field" DataField="Field" SortExpression="Field" />
		<asp:BoundField HeaderText="Language" DataField="Language" SortExpression="Language" />
		<asp:BoundField HeaderText="Value" SortExpression="Value" />
		<asp:BoundField HeaderText="Shared With" DataField="SharedWith" SortExpression="SharedWith" />
	</Columns>
	<EmptyDataTemplate>No Data</EmptyDataTemplate>
</asp:GridView>
<asp:ObjectDataSource ID="dsrcVersionDetails" runat="server" SelectMethod="GetEntityVersionValues" TypeName="ToSic.Eav.EavContext" OnObjectCreating="dsrcVersionDetails_ObjectCreating" OnSelecting="dsrcVersionDetails_Selecting">
	<SelectParameters>
		<asp:Parameter Name="entityId" Type="Int32" />
		<asp:Parameter Name="changeId" Type="Int32" />
		<asp:Parameter Name="defaultCultureDimension" Type="Int32" />
		<asp:Parameter Name="multiValuesSeparator" Type="String" />
	</SelectParameters>
</asp:ObjectDataSource>
<asp:Panel runat="server" ID="pnlActions">
	<asp:Hyperlink CssClass="eav-cancel" ID="hlkBack" runat="server" Text="Back" />
	<asp:LinkButton CssClass="eav-restore" ID="btnRestore" runat="server" Text="Restore" OnClick="btnRestore_Click" OnClientClick="return confirm('Are you sure you want to restore this Version?')" />
</asp:Panel>
