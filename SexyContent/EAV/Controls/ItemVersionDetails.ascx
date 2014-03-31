<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemVersionDetails.ascx.cs" Inherits="ToSic.Eav.ManagementUI.ItemVersionDetails" %>
<h1><asp:Literal runat="server" Text="Version {0} of {1} (Entity {2})" ID="litControlHeading"/></h1>
<h2>Result of changes</h2>
<asp:GridView runat="server" ID="grdVersionDetails" AllowSorting="True" DataSourceID="dsrcVersionDetails">
	<EmptyDataTemplate>No Data</EmptyDataTemplate>
</asp:GridView>
<asp:ObjectDataSource ID="dsrcVersionDetails" runat="server" SelectMethod="GetEntityVersionValues" TypeName="ToSic.Eav.EavContext" OnObjectCreating="dsrcVersionDetails_ObjectCreating" OnSelecting="dsrcVersionDetails_Selecting">
	<SelectParameters>
		<asp:Parameter Name="entityId" Type="Int32" />
		<asp:Parameter Name="changeId" Type="Int32" />
	</SelectParameters>
</asp:ObjectDataSource>
<h2>Changes to previous Version</h2>
<asp:GridView runat="server" ID="grdVersionChanges" AllowSorting="True" DataSourceID="dsrcVersionChanges">
	<EmptyDataTemplate>No Changes</EmptyDataTemplate>
</asp:GridView>
<asp:ObjectDataSource ID="dsrcVersionChanges" runat="server" OnObjectCreating="dsrcVersionChanges_ObjectCreating" OnSelecting="dsrcVersionChanges_Selecting" SelectMethod="GetEntityChangedValues" TypeName="ToSic.Eav.EavContext">
	<SelectParameters>
		<asp:Parameter Name="entityId" Type="Int32" />
		<asp:Parameter Name="changeId" Type="Int32" />
	</SelectParameters>
</asp:ObjectDataSource>
<asp:Panel runat="server" ID="pnlActions">
	<asp:Hyperlink CssClass="eav-cancel" ID="hlkBack" runat="server" Text="Cancel" />
	<asp:LinkButton CssClass="eav-restore" ID="btnRestore" runat="server" Text="Restore" OnClick="btnRestore_Click" />
</asp:Panel>
