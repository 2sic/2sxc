<%@ Control Language="C#" AutoEventWireup="True"
	Inherits="ToSic.Eav.ManagementUI.ContentTypeFields" Codebehind="ContentTypeFields.ascx.cs" %>

<asp:Panel ID="pnlNavigateBack" runat="server" Visible='<%# !IsDialog %>'>
	<asp:HyperLink ID="hlnkNavigateBack" NavigateUrl='<%# ReturnUrl %>' runat="server" Text="Back" />
</asp:Panel>
<asp:UpdatePanel ID="upnlAddEditField" runat="server">
	<ContentTemplate>
		<asp:LinkButton ID="lbtnInsertField" Text="Insert Field" runat="server" OnClick="lbtnInsertField_Click" />
		<asp:Panel runat="server" ID="pnlAddEditField" Visible="false">
			<asp:FormView ID="frvAddEditField" runat="server" DataSourceID="dsrcAttributes" DefaultMode="Insert"
				OnItemCommand="frvAddEditField_ItemCommand" OnItemInserted="frvAddEditField_ItemInserted"
				Width="100%" EnableViewState="False">
				<InsertItemTemplate>
					<table style="width: 100%">
						<tr>
							<th>
								<asp:Label runat="server" AssociatedControlID="StaticNameTextBox" Text="Static Name" />
								<asp:RequiredFieldValidator ID="rfvStaticName" runat="server" ControlToValidate="StaticNameTextBox"
									ValidationGroup="AddEditField" Text="*" SetFocusOnError="True" Display="Dynamic" />
							</th>
							<td>
								<asp:TextBox ID="StaticNameTextBox" runat="server" Text='<%# Bind("StaticName") %>'
									CssClass="StandardInput" ValidationGroup="AddEditField" />
								<asp:RegularExpressionValidator ID="revStaticName" runat="server" 
									ControlToValidate="StaticNameTextBox" ValidationGroup="AddEditField" 
									Text='<%# ToSic.Eav.EavContext.AttributeStaticNameRegExNotes %>' 
									ValidationExpression='<%# ToSic.Eav.EavContext.AttributeStaticNameRegEx %>' SetFocusOnError="True" Display="Dynamic" />
							</td>
						</tr>
						<tr>
							<th>
								<asp:Label runat="server" AssociatedControlID="TypeDropDownList" Text="Type" />
								<asp:RequiredFieldValidator ID="rfvType" Text="*" ControlToValidate="TypeDropDownList"
									runat="server" SetFocusOnError="True" Display="Dynamic" />
							</th>
							<td>
								<asp:DropDownList ID="TypeDropDownList" runat="server" DataSourceID="dsrcAttributeTypes"
									DataTextField="Type" DataValueField="Type" SelectedValue='<%# Bind("Type") %>'
									CssClass="StandardInput" ValidationGroup="AddEditField" 
									AppendDataBoundItems="true">
									<asp:ListItem Text="Please select..." Enabled="false" Value="" Selected="True" />
								</asp:DropDownList>
								<asp:EntityDataSource ID="dsrcAttributeTypes" runat="server" ConnectionString="name=EavContext"
									DefaultContainerName="EavContext" EnableFlattening="False" EntitySetName="AttributeTypes"
									EntityTypeFilter="AttributeType" oncontextcreating="dsrcAttributeTypes_ContextCreating" />
							</td>
						</tr>
					</table>
					<br />
					<asp:Button ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert"
						Text="Insert" ValidationGroup="AddEditField" />
					&nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False"
						CommandName="Cancel" Text="Cancel" ValidationGroup="AddEditField" />
				</InsertItemTemplate>
			</asp:FormView>
		</asp:Panel>
	</ContentTemplate>
</asp:UpdatePanel>
<asp:EntityDataSource ID="dsrcAttributeSet" runat="server" ConnectionString="name=EavContext"
	DefaultContainerName="EavContext" EnableFlattening="False" EntitySetName="AttributeSets"
	EntityTypeFilter="AttributeSet" Include="" 
	Where="it.AttributeSetId = @AttributeSetId" 
	oncontextcreating="dsrcAttributeSet_ContextCreating" OnSelected="dsrcAttributeSet_Selected">
	<WhereParameters>
		<asp:Parameter Name="AttributeSetId" Type="Int32" />
	</WhereParameters>
</asp:EntityDataSource>
<asp:ListView ID="lstvHeading" runat="server" DataSourceID="dsrcAttributeSet">
	<ItemTemplate>
		<h1>
			<asp:Label ID="lblHeading" runat="server" Text="Data Structure/Schema:" />&nbsp;<asp:Label
				ID="lblAttributeSetName" runat="server" Text='<%# Eval("Name") %>' /></h1>
	</ItemTemplate>
	<LayoutTemplate>
		<asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
	</LayoutTemplate>
</asp:ListView>
<asp:GridView ID="grdAttributesInSets" runat="server" AutoGenerateColumns="False"
	DataSourceID="dsrcAttributes" DataKeyNames="AttributeID" OnRowCommand="grdAttributesInSets_RowCommand"
	EnableViewState="False" OnDataBound="grdAttributesInSets_DataBound">
	<Columns>
		<asp:BoundField HeaderText="Static Name" DataField="StaticName" />
		<asp:BoundField DataField="Type" HeaderText="Type" />
		<asp:BoundField DataField="Name" HeaderText="Name" />
		<asp:BoundField DataField="Notes" HeaderText="Notes" HtmlEncode="False" />
		<asp:TemplateField HeaderText="is Title">
			<ItemTemplate>
				<asp:LinkButton ID="lbtnMakeTitle" Text="No" CommandName="MakeTitle" CommandArgument='<%# Eval("AttributeID") %>' Visible='<%# !(bool)Eval("IsTitle") %>' runat="server" />
				<asp:Label ID="lblIsTitle" Text="Yes" Visible='<%# (bool)Eval("IsTitle") %>' runat="server" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField HeaderText="Edit">
			<ItemTemplate>
				<asp:LinkButton ID="lbtnEditGeneralMetaData" Text="General" runat="server" CommandName="EditAllTypeMetaData" CommandArgument='<%# Eval("AttributeID") %>' />,&nbsp;<asp:LinkButton ID="lbtnEditTypeMetaData" Text='<%# Eval("Type") %>' runat="server" CommandName="EditTypeMetaData" CommandArgument='<%# Eval("AttributeID") %>' Visible='<%# Eval("HasTypeMetaData") %>' />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
				<asp:LinkButton
					ID="lbtnDelete" Visible='<%# !(bool)Eval("IsTitle") %>' Text="Delete" runat="server" CommandName="Delete" OnClientClick='return confirm("Are you sure you want to delete this entry?");'
					CommandArgument='<%# Eval("AttributeID") %>' />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
				<asp:LinkButton ID="lbtnMoveUp" Text="Up" runat="server" CommandName="MoveUp" CommandArgument='<%# Eval("AttributeID") %>'
					Visible='<%# Container.DisplayIndex > 0 %>' />
				<asp:LinkButton ID="lbtnMoveDown" Text="Down" runat="server" CommandName="MoveDown"
					CommandArgument='<%# Eval("AttributeID") %>' />
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
	<SelectedRowStyle BackColor="Gray" />
	<EmptyDataTemplate>
		No Records
	</EmptyDataTemplate>
</asp:GridView>
<asp:ObjectDataSource ID="dsrcAttributes" runat="server" SelectMethod="GetAttributesWithMetaInfo"
	TypeName="ToSic.Eav.EavContext" OnObjectCreating="dsrcAttributes_ObjectCreating"
	InsertMethod="AppendAttribute" OnInserting="dsrcAttributes_Inserting"
	DeleteMethod="RemoveAttributeInSet" OnDeleting="dsrcAttributes_Deleting"
	UpdateMethod="UpdateAttribute" OnInserted="dsrcAttributes_Inserted" 
	onselecting="dsrcAttributes_Selecting">
	<DeleteParameters>
		<asp:Parameter Name="attributeId" Type="Int32" />
		<asp:Parameter Name="attributeSetId" Type="Int32" />
	</DeleteParameters>
	<InsertParameters>
		<asp:Parameter Name="attributeSetId" Type="Int32" />
		<asp:Parameter Name="type" Type="String" />
		<asp:Parameter Name="isTitle" Type="Boolean" DefaultValue="False" />
	</InsertParameters>
	<SelectParameters>
		<asp:Parameter Name="attributeSetId" Type="Int32" />
		<asp:Parameter Name="dimensionIds" Type="Object" />
	</SelectParameters>
	<UpdateParameters>
		<asp:Parameter Name="attributeID" Type="Int32" />
		<asp:Parameter Name="isTitle" Type="Boolean" DefaultValue="False" />
	</UpdateParameters>
</asp:ObjectDataSource>

