<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="x AppManagement.ascx.cs" Inherits="ToSic.SexyContent.Administration.AppManagement" %>
<%@ Import Namespace="ToSic.SexyContent" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>


<div class="dnnForm dnnSexyContentManageApps dnnClear">
    <h2 class="dnnFormSectionHead">
        <asp:Label runat="server" ResourceKey="lblManageAppsHeading"></asp:Label>
    </h2>
    <fieldset>
        <dnnweb:DnnGrid CssClass="GridApps" ID="grdApps" runat="server" AutoGenerateColumns="false" EnableEmbeddedSkins="True" EnableEmbeddedBaseStylesheet="True" Skin="Default" EnableViewState="true" OnDeleteCommand="grdApps_DeleteCommand" OnNeedDataSource="grdApps_NeedDataSource" OnItemDataBound="grdApps_ItemDataBound">
            <ClientSettings><ClientEvents OnCommand="AppDeleting" /></ClientSettings>
            <mastertableview datakeynames="AppId" allowsorting="True" headerstyle-font-bold="true">
                <Columns>
                    <dnnweb:DnnGridButtonColumn UniqueName="DeleteColumn" ButtonType="ImageButton" ImageUrl="~/Images/Delete.gif" CommandName="delete" ButtonCssClass="sc-app-delete">
                        <HeaderStyle Width="35px" />
                    </dnnweb:DnnGridButtonColumn>
                    <dnnweb:DnnGridBoundColumn HeaderText="AppId" DataField="AppId"></dnnweb:DnnGridBoundColumn>
                    <dnnweb:DnnGridTemplateColumn HeaderText="Name" DataField="Name" UniqueName="Name">
                        <ItemTemplate>
                            <div style='<%# (Eval("Name") != "Content") ? "display:block;" : "display:none;" %>'>
                                <a data-app-name='<%# HttpUtility.HtmlEncode(Eval("Name")) %>' href="<%# new SexyContent(ZoneId.Value, (int)Eval("AppID")).GetMetaDataEditUrl(TabId, ModuleId, Request.RawUrl, this, SexyContent.AttributeSetStaticNameApps, SexyContent.AssignmentObjectTypeIDSexyContentApp, (int)Eval("AppID")) %>">
                                    <%# Eval("Name") == null ? "(error)" : HttpUtility.HtmlEncode(Eval("Name")) %>
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
                            <%# ((dynamic)Eval("Configuration")) == null || ((dynamic)Eval("Configuration")).AllowTokenTemplates == null ? "-" : ((dynamic)Eval("Configuration")).AllowTokenTemplates.ToString() %>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    <dnnweb:DnnGridTemplateColumn HeaderText="RazorTemplates">
                        <ItemTemplate>
                            <%# ((dynamic)Eval("Configuration")) == null || ((dynamic)Eval("Configuration")).AllowRazorTemplates == null ? "-" : ((dynamic)Eval("Configuration")).AllowRazorTemplates.ToString() %>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    
                    <dnnweb:DnnGridTemplateColumn HeaderText="Settings">
                        <ItemTemplate>
                            <div style='<%# (Eval("Name") != "Content") ? "display:block;" : "display:none;" %>'>
                                <a href="<%# SexyContent.GetDefaultAppId(ZoneId.Value) == (int)Eval("AppID") ? "" : EditUrl("", "", SexyContent.ControlKeys.EavManagement, SexyContent.AppIDString + "=" + Eval("AppID") + "&" +
                                    "ManagementMode=ContentTypeFields&AttributeSetId=" + SexyContent.GetAppSettingsAttributeSetId(ZoneId.Value, (int)Eval("AppID"))) %>">Conf</a>
                                <a href="<%# SexyContent.GetDefaultAppId(ZoneId.Value) == (int)Eval("AppID") ? "" : new SexyContent(ZoneId.Value, (int)Eval("AppID")).GetMetaDataEditUrl(TabId, ModuleId, Request.RawUrl, this, SexyContent.AttributeSetStaticNameAppSettings,
                                    SexyContent.AssignmentObjectTypeIDSexyContentApp, (int)Eval("AppID")) %>">Edit</a>
                            </div>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    
                    <dnnweb:DnnGridTemplateColumn HeaderText="Resources">
                        <ItemTemplate>
                            <div style='<%# (Eval("Name") != "Content") ? "display:block;" : "display:none;" %>'>
                                <a href="<%# SexyContent.GetDefaultAppId(ZoneId.Value) == (int)Eval("AppID") ? "" :  EditUrl("", "", SexyContent.ControlKeys.EavManagement, SexyContent.AppIDString + "=" + Eval("AppID") + "&" +
                                    "ManagementMode=ContentTypeFields&AttributeSetId=" + SexyContent.GetAppResourcesAttributeSetId(ZoneId.Value, (int)Eval("AppID"))) %>">Conf</a>
                                <a href="<%# SexyContent.GetDefaultAppId(ZoneId.Value) == (int)Eval("AppID") ? "" :  new SexyContent(ZoneId.Value, (int)Eval("AppID")).GetMetaDataEditUrl(TabId, ModuleId, Request.RawUrl, this, SexyContent.AttributeSetStaticNameAppResources,
                                    SexyContent.AssignmentObjectTypeIDSexyContentApp, (int)Eval("AppID")) %>">Edit</a>
                            </div>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>

                    <dnnweb:DnnGridTemplateColumn HeaderText="ManageApp">
                        <ItemTemplate>
                            <a href="<%# EditUrl("", "", SexyContent.ControlKeys.GettingStarted, SexyContent.AppIDString + "=" + Eval("AppID")) %>">Manage</a>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    
                    <dnnweb:DnnGridCheckBoxColumn ReadOnly="true" DataField="Hidden" HeaderText="Hidden">
                        <HeaderStyle Width="80px" />
                    </dnnweb:DnnGridCheckBoxColumn>
                    
                    <dnnweb:DnnGridTemplateColumn HeaderText="ExportApp">
                        <ItemTemplate>
                            <a href="<%# EditUrl("", "", SexyContent.ControlKeys.AppExport, SexyContent.AppIDString + "=" + Eval("AppID")) %>">Export</a>
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
            <li><asp:Hyperlink ID="hlkBrowseApps" runat="server" CssClass="dnnPrimaryAction" ResourceKey="hlkBrowseApps" Target="_blank" NavigateUrl="http://2sxc.org/apps"></asp:Hyperlink></li>
            <li><a class="dnnSecondaryAction" href="<%= EditUrl("", "", SexyContent.ControlKeys.AppImport) %>">Import App</a></li>
            <li><asp:LinkButton ID="btnCreateApp" runat="server" CssClass="dnnSecondaryAction sc-create-app" ResourceKey="btnCreateApp" OnClientClick="return CreateApp()" OnClick="btnCreateApp_Click"></asp:LinkButton></li>
        </ul>
        
    </fieldset>
</div>

<asp:HiddenField runat="server" ID="hfNewAppName" />
<asp:HiddenField runat="server" ID="hfAppNameToDelete" />

<script type="text/javascript">
    var appNamePattern = /^[a-zA-Z0-9 -_]+$/g;

    function CreateApp() {
        var newName = "";
        do {
            newName = prompt("Enter a name for the new App", '');
            if (newName == null || newName == false || !(appNamePattern.test(newName))) {
                alert("App name cannot contain special characters.");
                return false;
            }
        } while(newName == "")
        $("#<%= hfNewAppName.ClientID %>").val(newName);
        return true;
    }

    function AppDeleting(sender, args) {
        if (args.get_commandName() == "delete") {
            args.set_cancel(true);

            var grid = sender;
            var masterTable = grid.get_masterTableView();
            var row = masterTable.get_dataItems()[args.get_commandArgument()];
            var cell = masterTable.getCellByColumnUniqueName(row, "Name");
            var appName = $(cell).find("a[data-app-name]").attr("data-app-name");
            //here cell.innerHTML holds the value of the cell  

            var deleteapp = false;
            var appNameConfirm = prompt('<%= LocalizeString("DeleteApp.ConfirmText") %>');

            // Only delete the app if the app name has been confirmed
            if (appName == appNameConfirm) {
                deleteapp = true;
                $("#<%= hfAppNameToDelete.ClientID %>").val(appName);
            }

            args.set_cancel(!deleteapp);
        }
    }

</script>

<style type="text/css">
    .aspNetDisabled .sc-app-delete { display: none; }
</style>