<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppManagement.ascx.cs" Inherits="ToSic.SexyContent.Administration.AppManagement" %>
<%@ Import Namespace="ToSic.SexyContent" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="Telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>


<div class="dnnForm dnnSexyContentManageApps dnnClear">
    <h2 class="dnnFormSectionHead">
        <asp:Label runat="server" ResourceKey="lblManageAppsHeading"></asp:Label>
    </h2>
    <fieldset>
        <dnnweb:DnnGrid CssClass="GridApps" ID="grdApps" runat="server" AutoGenerateColumns="false" EnableEmbeddedSkins="True" EnableEmbeddedBaseStylesheet="True" Skin="Default" EnableViewState="true" OnDeleteCommand="grdApps_DeleteCommand" OnNeedDataSource="grdApps_NeedDataSource" OnItemDataBound="grdApps_ItemDataBound">
            <ClientSettings><ClientEvents OnCommand="AppDeleting" /></ClientSettings>
            <mastertableview datakeynames="AppID" allowsorting="True" headerstyle-font-bold="true">
                <Columns>
                    <dnnweb:DnnGridButtonColumn UniqueName="DeleteColumn" ButtonType="ImageButton" ImageUrl="~/Images/Delete.gif" CommandName="delete" ButtonCssClass="sc-app-delete">
                        <HeaderStyle Width="35px" />
                    </dnnweb:DnnGridButtonColumn>
                    <dnnweb:DnnGridTemplateColumn HeaderText="ContentType" DataField="Name">
                        <ItemTemplate>
                            <div style='<%# (Eval("Name") != "Content") ? "display:block;" : "display:none;" %>'>
                                <a href="<%# SexyContent.GetMetaDataEditUrl(TabId, ModuleId, Request.RawUrl, this, SexyContent.AttributeSetStaticNameApps, SexyContent.AssignmentObjectTypeIDSexyContentApp, (int)Eval("AppID"), ZoneId.Value, (int)Eval("AppID")) %>">
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
                    
                    <dnnweb:DnnGridTemplateColumn HeaderText="Settings">
                        <ItemTemplate>
                            <div style='<%# (Eval("Name") != "Content") ? "display:block;" : "display:none;" %>'>
                                <a href="<%# SexyContent.GetDefaultAppId(ZoneId.Value) == (int)Eval("AppID") ? "" : EditUrl("", "", SexyContent.ControlKeys.EavManagement, SexyContent.AppIDString + "=" + Eval("AppID") + "&" +
                                    "ManagementMode=ContentTypeFields&AttributeSetId=" + SexyContent.GetAppSettingsAttributeSetId(ZoneId.Value, (int)Eval("AppID"))) %>">Conf</a>
                                <a href="<%# SexyContent.GetDefaultAppId(ZoneId.Value) == (int)Eval("AppID") ? "" : SexyContent.GetMetaDataEditUrl(TabId, ModuleId, Request.RawUrl, this, SexyContent.AttributeSetStaticNameAppSettings,
                                    SexyContent.AssignmentObjectTypeIDSexyContentApp, (int)Eval("AppID"), ZoneId.Value, (int)Eval("AppID")) %>">Edit</a>
                            </div>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    
                    <dnnweb:DnnGridTemplateColumn HeaderText="Resources">
                        <ItemTemplate>
                            <div style='<%# (Eval("Name") != "Content") ? "display:block;" : "display:none;" %>'>
                                <a href="<%# SexyContent.GetDefaultAppId(ZoneId.Value) == (int)Eval("AppID") ? "" :  EditUrl("", "", SexyContent.ControlKeys.EavManagement, SexyContent.AppIDString + "=" + Eval("AppID") + "&" +
                                    "ManagementMode=ContentTypeFields&AttributeSetId=" + SexyContent.GetAppResourcesAttributeSetId(ZoneId.Value, (int)Eval("AppID"))) %>">Conf</a>
                                <a href="<%# SexyContent.GetDefaultAppId(ZoneId.Value) == (int)Eval("AppID") ? "" :  SexyContent.GetMetaDataEditUrl(TabId, ModuleId, Request.RawUrl, this, SexyContent.AttributeSetStaticNameAppResources,
                                    SexyContent.AssignmentObjectTypeIDSexyContentApp, (int)Eval("AppID"), ZoneId.Value, (int)Eval("AppID")) %>">Edit</a>
                            </div>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>

                    <dnnweb:DnnGridTemplateColumn HeaderText="ManageApp">
                        <ItemTemplate>
                            <a href="<%# EditUrl("", "", SexyContent.ControlKeys.EavManagement, SexyContent.AppIDString + "=" + Eval("AppID")) %>">Manage</a>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    
                    <dnnweb:DnnGridCheckBoxColumn ReadOnly="true" DataField="Hidden" HeaderText="Hidden">
                        <HeaderStyle Width="80px" />
                    </dnnweb:DnnGridCheckBoxColumn>
                    
                    <dnnweb:DnnGridTemplateColumn HeaderText="ExportApp">
                        <ItemTemplate>
                            <div style='<%# (Eval("Name") != "Content") ? "display:block;" : "display:none;" %>'>
                                <a href="<%# EditUrl("", "", SexyContent.ControlKeys.AppExport, SexyContent.AppIDString + "=" + Eval("AppID")) %>">Export</a>
                            </div>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>

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
            <li><a class="dnnSecondaryAction" href="<%= EditUrl("", "", SexyContent.ControlKeys.AppImport) %>">Import App</a></li>
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

    function AppDeleting(sender, args) {
        var deleteapp = false;
        if (eventArgs.get_commandName() == "delete")
            confirm('<%= LocalizeString("DeleteApp.Confirm") %>');
        args.set_cancel(!deleteapp);
    }

</script>

<style type="text/css">
    .aspNetDisabled .sc-app-delete { display: none; }
</style>