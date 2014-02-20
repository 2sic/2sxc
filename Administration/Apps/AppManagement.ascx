<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppManagement.ascx.cs" Inherits="ToSic.SexyContent.Administration.AppManagement" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="Telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<div class="dnnForm dnnSexyContentManageApps dnnClear">
    <h2 class="dnnFormSectionHead">
        <asp:Label runat="server" ResourceKey="lblManageAppsHeading"></asp:Label>
    </h2>
    <fieldset>
        <dnnweb:DnnGrid CssClass="GridApps" ID="grdApps" runat="server" AutoGenerateColumns="false" EnableEmbeddedSkins="True" EnableEmbeddedBaseStylesheet="True" Skin="Default" EnableViewState="true" OnNeedDataSource="grdApps_NeedDataSource" OnItemDataBound="grdApps_ItemDataBound">
            <mastertableview datakeynames="AppID" allowsorting="True" headerstyle-font-bold="true">
                <Columns>
                    <dnnweb:DnnGridButtonColumn UniqueName="DeleteColumn" ButtonType="ImageButton" ImageUrl="~/Images/Delete.gif" CommandName="delete">
                        <HeaderStyle Width="35px" />
                    </dnnweb:DnnGridButtonColumn>
                    <dnnweb:DnnGridTemplateColumn HeaderText="ContentType" DataField="Name">
                        <ItemTemplate>
                            <div style='<%# (Eval("Name") != "Content") ? "display:block;" : "display:none;" %>'>
                                <a href="<%# Sexy.GetMetaDataEditUrl(TabId, ModuleId, Request.RawUrl, this, ToSic.SexyContent.SexyContent.AttributeSetStaticNameApps, Sexy.AssignmentObjectTypeIDSexyContentApp, (int)Eval("AppID"), (int)Eval("AppID")) %>">
                                    <%# HttpUtility.HtmlEncode(Eval("Name")) %>
                                </a>
                            </div>
                            <div style='<%# (Eval("Name") == "Content") ? "display:block;" : "display:none;" %>'>
                                Content
                            </div>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    <dnnweb:DnnGridBoundColumn HeaderText="Folder" DataField="Folder"></dnnweb:DnnGridBoundColumn>
                    <dnnweb:DnnGridTemplateColumn HeaderText="TokenTemplates">
                        <ItemTemplate>
                            <%# ((dynamic)Eval("Configuration")) == null ? "-" : ((dynamic)Eval("Configuration")).AllowTokenTemplates.ToString() %>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    <dnnweb:DnnGridTemplateColumn HeaderText="RazorTemplates">
                        <ItemTemplate>
                            <%# ((dynamic)Eval("Configuration")) == null ? "-" : ((dynamic)Eval("Configuration")).AllowRazorTemplates.ToString() %>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>


                    <dnnweb:DnnGridTemplateColumn HeaderText="ManageApp">
                        <ItemTemplate>
                            <a href="<%# EditUrl("", "", "eavmanagement", ToSic.SexyContent.SexyContent.AppIDString + "=" + Eval("AppID")) %>">Manage</a>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>

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
            <li><asp:LinkButton ID="btnCreateApp" runat="server" CssClass="dnnSecondaryAction sc-create-app" ResourceKey="btnCreateApp" OnClientClick="return CreateApp()" OnClick="btnCreateApp_Click"></asp:LinkButton></li>
        </ul>
        
    </fieldset>
</div>

<asp:HiddenField runat="server" ID="hfNewAppName" />

<script type="text/javascript">
    function CreateApp() {
        var newName = "";
        do {
            newName = prompt("Enter a name for the new App", '');
            if (newName == null || newName == false)
                return false;
        } while(newName == "")
        $("#<%= hfNewAppName.ClientID %>").val(newName);
        return true;
    }
</script>