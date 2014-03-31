<%@ Control Language="C#" AutoEventWireup="True"
	Inherits="ToSic.Eav.ManagementUI.ContentTypesList" Codebehind="ContentTypesList.ascx.cs" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
	<ContentTemplate>
		<asp:EntityDataSource ID="dsrcAttributeSets" runat="server" ConnectionString="name=EavContext"
			DefaultContainerName="EavContext" EntitySetName="AttributeSets"
			EnableDelete="True" EnableViewState="False" EnableFlattening="False" StoreOriginalValuesInViewState="False"
			Include="AttributesInSets,Entities" OrderBy="it.Name" 
			OnContextCreating="dsrcAttributeSets_ContextCreating" 
			OnDeleting="dsrcAttributeSets_Deleting" 
			Where="it.ChangeLogIDDeleted IS NULL AND (it.Scope = @Scope OR @Scope IS NULL) AND it.AppId = @AppId" 
			onselecting="dsrcAttributeSets_Selecting">
			<WhereParameters>
				<asp:Parameter Name="Scope" Size="50" Type="String" />
				<asp:Parameter Name="AppId" Type="Int32" />
			</WhereParameters>
		</asp:EntityDataSource>
		<asp:QueryExtender ID="qeAttributeSets" runat="server" TargetControlID="dsrcAttributeSets">
			<asp:SearchExpression SearchType="Contains" DataFields="Name,Description">
				<asp:ControlParameter ControlID="SearchTextBox" />
			</asp:SearchExpression>
		</asp:QueryExtender>
		Filter:
		<asp:TextBox runat="server" ID="SearchTextBox" AutoPostBack="true" />
		<asp:LinkButton Text="Refresh" runat="server" ID="lbtnRefreshData" OnClick="lbtnRefreshData_Click"
			ClientIDMode="Static" Style="display: block" />
		<asp:GridView ID="grdAttributeSets" runat="server" AutoGenerateColumns="False" DataSourceID="dsrcAttributeSets"
			DataKeyNames="AttributeSetID" CssClass="EAVAttributeSets">
			<Columns>
				<asp:BoundField DataField="AttributeSetId" HeaderText="AttributeSetId" Visible="false" />
				<asp:BoundField DataField="Name" HeaderText="Name" />
				<asp:BoundField DataField="Description" HeaderText="Description" />
				<asp:TemplateField HeaderText="Fields">
					<ItemTemplate>
						<asp:Label ID="lblFieldsCount" Text='<%# Eval("AttributesInSets.Count") %>' runat="server" />&nbsp;<asp:HyperLink
							ID="hlnkDesignFields" NavigateUrl='<%# GetDesignFieldsUrl(Eval("AttributeSetId")) %>'
							Text="Design" CssClass='<%# UseDialogs ? "Dialog" : "" %>' runat="server" />
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Records">
					<ItemTemplate>
						<asp:Label ID="lblRecordsCount" runat="server" Text='<%# ((System.Data.Objects.DataClasses.EntityCollection<ToSic.Eav.Entity>)Eval("Entities")).Count(en => !en.ChangeLogIDDeleted.HasValue) %>' />
						<asp:HyperLink ID="hlnkShowItems" NavigateUrl='<%# GetShowItemsUrl(Eval("AttributeSetId")) %>'
							runat="server" CssClass='<%# UseDialogs ? "Dialog" : "" %>' Text="Show Items" />
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField>
					<ItemTemplate>
						<asp:HyperLink ID="hlnkConfigureContentType" NavigateUrl='<%# GetConfigureContentTypeUrl(Eval("AttributeSetId")) %>'
							Text="Edit" CssClass='<%# UseDialogs ? "Dialog" : "" %>' runat="server" />&nbsp;<asp:LinkButton ID="lbtnDeleteField"
								Text="Delete" runat="server" CommandName="Delete" OnClientClick='return confirm("Are you sure you want to delete this entry?");' />
					</ItemTemplate>
				</asp:TemplateField>
			</Columns>
			<RowStyle VerticalAlign="Top" />
		</asp:GridView>
	</ContentTemplate>
</asp:UpdatePanel>
<asp:HyperLink ID="hlnkNewContentType" NavigateUrl='<%# GetNewContentTypeUrl() %>' runat="server" CssClass='<%# UseDialogs ? "Dialog" : "" %>' Text="New Content Type" />