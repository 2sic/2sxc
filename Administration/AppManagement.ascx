<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppManagement.ascx.cs" Inherits="ToSic.SexyContent.Administration.AppManagement" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="Telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<div class="dnnForm dnnSexyContentManageApps dnnClear">
    <h2 class="dnnFormSectionHead">
        <asp:Label runat="server" ResourceKey="lblManageAppsHeading"></asp:Label>
    </h2>
    <fieldset>
        <dnnweb:DnnGrid CssClass="GridApps" ID="grdApps" runat="server" AutoGenerateColumns="false" EnableEmbeddedSkins="True" EnableEmbeddedBaseStylesheet="True" Skin="Default" EnableViewState="true" OnNeedDataSource="grdApps_NeedDataSource">
            <mastertableview datakeynames="AppID" allowsorting="True" headerstyle-font-bold="true">
                <Columns>
                    <dnnweb:DnnGridButtonColumn UniqueName="DeleteColumn" ButtonType="ImageButton" ImageUrl="~/Images/Delete.gif" CommandName="delete">
                        <HeaderStyle Width="35px" />
                    </dnnweb:DnnGridButtonColumn>
                    <dnnweb:DnnGridButtonColumn ButtonType="ImageButton" ImageUrl="~/Images/Edit.gif" CommandName="edit">
                        <HeaderStyle Width="35px" />
                    </dnnweb:DnnGridButtonColumn>
                    <dnnweb:DnnGridBoundColumn HeaderText="AppName" DataField="Name"></dnnweb:DnnGridBoundColumn>
                    <dnnweb:DnnGridBoundColumn HeaderText="Folder" DataField="Name"></dnnweb:DnnGridBoundColumn>
                    <dnnweb:DnnGridBoundColumn HeaderText="Templates" DataField="Name"></dnnweb:DnnGridBoundColumn>

                    <%--<dnnweb:DnnGridTemplateColumn HeaderText="TemplatePath" DataField="TemplatePath">
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
                        <HeaderStyle Width="80px" />
                    </dnnweb:DnnGridCheckBoxColumn>--%>
                </Columns>
                <NoRecordsTemplate>
                    <asp:Label ID="lblNoRecords" runat="server" resourcekey="lblNoRecords"></asp:Label>
                </NoRecordsTemplate>
            </mastertableview>
            <clientsettings>
                <Scrolling AllowScroll="False">
                </Scrolling>
            </clientsettings>
        </dnnweb:DnnGrid>
        <ul class="dnnActions dnnClear">
            <li><asp:Hyperlink ID="hlkBrowseApps" runat="server" CssClass="dnnPrimaryAction" ResourceKey="hlkBrowseApps"></asp:Hyperlink></li>
            <li><asp:Hyperlink ID="hlkImportApp" runat="server" CssClass="dnnSecondaryAction" ResourceKey="hlkImportApp"></asp:Hyperlink></li>
            <li><asp:Hyperlink ID="hlkCreateApp" runat="server" CssClass="dnnSecondaryAction" ResourceKey="hlkCreateApp"></asp:Hyperlink></li>
        </ul>
    </fieldset>
</div>