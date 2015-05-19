<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.ManageTemplates" Codebehind="ManageTemplates.ascx.cs" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="Telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>
<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

<asp:Panel runat="server" class="dnnForm dnnSexyContentManageTemplates dnnClear" id="pnlManageTemplates">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelManageTemplates"><asp:Label runat="server" ID="lblManageTemplatesHeading" ResourceKey="lblManageTemplatesHeading"></asp:Label></h2>
    <asp:Panel runat="server" ID="pnlSexyContentDesignersInfo" Visible="false" CssClass="dnnFormMessage dnnFormInfo" >
        <asp:Label runat="server" ResourceKey="lblSexyContentDesignersInfo" ID="lblSexyContentDesignersInfo"></asp:Label>
    </asp:Panel>
    <fieldset>
        <dnnweb:DnnGrid CssClass="GridTemplates" ID="grdTemplates" runat="server" AutoGenerateColumns="false" EnableEmbeddedSkins="True" EnableEmbeddedBaseStylesheet="True" Skin="Default" OnDeleteCommand="grdTemplates_DeleteCommand" OnSortCommand="grdTemplates_SortCommand" OnEditCommand="grdTemplates_EditCommand" EnableViewState="true">
            <MasterTableView DataKeyNames="TemplateID, Guid" AllowSorting="True" HeaderStyle-Font-Bold="true">
                <GroupByExpressions>
                    <Telerik:GridGroupByExpression>
                        <SelectFields>
                            <Telerik:GridGroupByField FieldName="AttributeSetName" meta:resourcekey="ContentTypeGroupByField" HeaderText="Content Type" />
                        </SelectFields>
                        <GroupByFields>
                            <Telerik:GridGroupByField FieldName="AttributeSetName" SortOrder="Ascending" />
                        </GroupByFields>
                    </Telerik:GridGroupByExpression>
                </GroupByExpressions>
                <Columns>
                    <dnnweb:DnnGridButtonColumn UniqueName="DeleteColumn" ButtonType="ImageButton" ImageUrl="~/Images/Delete.gif" CommandName="delete">
                        <HeaderStyle Width="35px" />
                    </dnnweb:DnnGridButtonColumn>
                    <dnnweb:DnnGridButtonColumn ButtonType="ImageButton" ImageUrl="~/Images/Edit.gif" CommandName="edit">
                        <HeaderStyle Width="35px" />
                    </dnnweb:DnnGridButtonColumn>
                    <dnnweb:DnnGridBoundColumn HeaderText="TemplateName" DataField="TemplateName"></dnnweb:DnnGridBoundColumn>
                    <dnnweb:DnnGridTemplateColumn HeaderText="TemplatePath" DataField="TemplatePath">
                        <HeaderStyle Width="160px" />
                        <ItemTemplate>
                            <span title='<%# HttpUtility.HtmlEncode(Eval("TemplatePath")) %>' class="NoWrapFixedWidth"><%# HttpUtility.HtmlEncode(Eval("TemplatePath")) %></span>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    <dnnweb:DnnGridTemplateColumn HeaderText="ContentType" DataField="AttributeSetName">
                        <ItemTemplate>
                            <span title='<%# HttpUtility.HtmlEncode(Eval("AttributeSetName")) %>' class="NoWrapFixedWidth"><%# HttpUtility.HtmlEncode(Eval("AttributeSetName")) %></span>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    <dnnweb:DnnGridBoundColumn HeaderText="DemoRow" DataField="DemoEntityID">
                        <HeaderStyle Width="110px" />
                    </dnnweb:DnnGridBoundColumn>
                    <dnnweb:DnnGridCheckBoxColumn ReadOnly="true" DataField="IsHidden" HeaderText="IsHidden">
                        <HeaderStyle Width="60px" />
                    </dnnweb:DnnGridCheckBoxColumn>
                    <dnnweb:DnnGridBoundColumn HeaderText="Url" DataField="ViewNameInUrl">
                        <HeaderStyle Width="100px" />
                    </dnnweb:DnnGridBoundColumn>
                    <dnnweb:DnnGridTemplateColumn HeaderText="Permissions" >
                        <HeaderStyle Width="35px" />
                        <ItemTemplate>
                            <a href="<%# PermissionsLink(Eval("Guid").ToString()) %>">Permissions</a>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>

                </Columns>
                <NoRecordsTemplate>
                    <asp:Label ID="lblNoRecords" runat="server" resourcekey="lblNoRecords"></asp:Label>
                </NoRecordsTemplate>
            </MasterTableView>
             <ClientSettings>
                <Scrolling AllowScroll="False">
                </Scrolling>
            </ClientSettings>
        </dnnweb:DnnGrid>
        <ul class="dnnActions dnnClear">
            <li><asp:HyperLink ID="hlkCancel" runat="server" CssClass="dnnPrimaryAction" ResourceKey="hlkCancel"></asp:HyperLink></li>
            <li><asp:HyperLink ID="hlkNewTemplate" runat="server" CssClass="dnnSecondaryAction" ResourceKey="hlkNewTemplate"></asp:HyperLink></li>
        </ul>
    </fieldset>
</asp:Panel>

<style type="text/css">
    .NoWrapFixedWidth { width:150px; white-space:nowrap; overflow:hidden; display:block; text-overflow:ellipsis; }
    .rgDataDiv { height: auto !important; }
    .RadGrid .rgRow td, .RadGrid .rgAltRow td, .RadGrid .rgEditRow td, .RadGrid .rgFooter td { padding: 4px 7px 3px 7px; vertical-align: middle; }
    .RadGrid .rgHeader, .RadGrid th.rgResizeCol { font-weight: normal; padding: 4px 7px 3px 7px; text-align: left; line-height:normal; }
    .RadGrid_Default .rgRow td, .RadGrid_Default .rgAltRow td, .RadGrid_Default .rgEditRow td, .RadGrid_Default .rgFooter td { border:none; }
</style>