<%@ Control Language="C#" AutoEventWireup="True" Inherits="ToSic.Eav.ManagementUI.ContentTypeEdit" Codebehind="ContentTypeEdit.ascx.cs" %>
<asp:FormView ID="frvAttributeSet" runat="server" DataSourceID="dsrcAttributeSet"
	DefaultMode="Insert" Width="100%" DataKeyNames="AttributeSetID" OnItemInserted="frvAttributeSet_ItemInserted"
	OnItemUpdated="frvAttributeSet_ItemUpdated" 
	onitemcommand="frvAttributeSet_ItemCommand">
	<EditItemTemplate>
		Name:<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="NameTextBox"
			Display="Dynamic" Text="*" ForeColor="#B71717" /><br />
		<asp:TextBox ID="NameTextBox" runat="server" Text='<%# Bind("Name") %>' CssClass="StandardInput" MaxLength="150" />
		<br />
		Description:<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
			ControlToValidate="DescriptionTextBox" Display="Dynamic" Text="*" ForeColor="#B71717" /><br />
		<asp:TextBox ID="DescriptionTextBox" runat="server" TextMode="MultiLine" Text='<%# Bind("Description") %>'
			CssClass="StandardInput" />
		<br />
		<asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
			Text="Update" />
		<asp:LinkButton ID="CancelButton" runat="server" CausesValidation="false" CommandName="Cancel"
			Text="Cancel" Visible='<%# !IsDialog %>' />
	</EditItemTemplate>
	<InsertItemTemplate>
		Name:<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="NameTextBox2"
			Display="Dynamic" Text="*" ForeColor="#B71717" /><br />
		<asp:TextBox ID="NameTextBox2" runat="server" Text='<%# Bind("Name") %>' CssClass="StandardInput" />
		<br />
		Description:<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
			ControlToValidate="DescriptionTextBox2" Display="Dynamic" Text="*" ForeColor="#B71717"
			CssClass="StandardInput" /><br />
		<asp:TextBox ID="DescriptionTextBox2" runat="server" TextMode="MultiLine" Text='<%# Bind("Description") %>'
			CssClass="StandardInput" />
		<br />
		<asp:Button ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert"
			Text="Insert" />
		<asp:LinkButton ID="CancelButton" runat="server" CausesValidation="false" CommandName="Cancel"
			Text="Cancel" Visible='<%# !IsDialog %>' />
	</InsertItemTemplate>
	<EmptyDataTemplate>
		Invalid AttributeSetId</EmptyDataTemplate>
</asp:FormView>
<asp:EntityDataSource ID="dsrcAttributeSet" runat="server" ConnectionString="name=EavContext"
	DefaultContainerName="EavContext" EnableFlattening="False"
	EnableInsert="True" EnableUpdate="True" EntitySetName="AttributeSets" EntityTypeFilter="AttributeSet"
	AutoGenerateWhereClause="True" Select="" Where="" 
	OnContextCreating="dsrcAttributeSet_ContextCreating" 
	OnInserting="dsrcAttributeSet_Inserting">
	<WhereParameters>
		<asp:QueryStringParameter Name="AttributeSetID" QueryStringField="AttributeSetId"
			Type="Int32" />
	</WhereParameters>
</asp:EntityDataSource>
<asp:Panel ID="pnlCloseDialog" runat="server" Visible="false">
	<script type="text/javascript">
		parent.CloseDialog();
	</script>
</asp:Panel>
