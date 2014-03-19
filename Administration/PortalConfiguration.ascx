<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PortalConfiguration.ascx.cs" Inherits="ToSic.SexyContent.Configuration.PortalConfiguration" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>
<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

<div class="dnnForm scPortalConfiguration dnnClear">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelSexyContentZones">
        <a href="#"><asp:Label runat="server" ID="lblZones" ResourceKey="lblZonesHeading"></asp:Label></a>
    </h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label runat="server" id="lblZone" Suffix=":"></dnn:Label>
            <asp:DropDownList Width="200px" runat="server" ID="ddlZones" DataTextField="Name" DataValueField="ZoneID" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlZones_SelectedIndexChanged"></asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label runat="server"></dnn:Label>
            <asp:LinkButton runat="server" ID="btnRenameZone" CssClass="dnnSecondaryAction" resourcekey="btnRenameZone" OnClientClick=" return RenameZone()" OnClick="btnRenameZone_Click"></asp:LinkButton>
            <asp:LinkButton runat="server" ID="btnCreateZone" CssClass="dnnSecondaryAction" resourcekey="btnCreateZone" OnClientClick="return CreateZone()" OnClick="btnCreateZone_Click"></asp:LinkButton>
            <asp:HiddenField runat="server" ID="hfZoneName" Value="" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label runat="server"></dnn:Label>
            <div class="dnnLeft" style="width:450px; margin-top:20px;">
                <asp:Literal runat="server" ID="litZoneInfo"></asp:Literal>
            </div>
        </div>
    </fieldset>

    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelSexyContentCultures">
        <a href="#"><asp:Label runat="server" ID="lblCulturesHeading" ResourceKey="lblCulturesHeading" Text="Cultures"></asp:Label></a>
    </h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label runat="server" id="lblCultures" resourcekey="lblCultures" Suffix=":"></dnn:Label>
            <div class="dnnLeft" style="width:450px">
                <asp:Panel Visible="true" runat="server" id="pnlSpecifyZoneFirst" CssClass="dnnFormInfo dnnFormMessage"><asp:Label runat="server" ID="lblSpecifyZoneFirst" resourcekey="lblSpecifyZoneFirst"></asp:Label></asp:Panel>
                <dnnweb:dnngrid ID="grdCultures" Width="400px" runat="server" AutoGenerateColumns="false" EnableViewState="true" OnItemCommand="grdCultures_ItemCommand" OnNeedDatasource="grdCultures_NeedDatasource" Visible="True">
                    <MasterTableView HeaderStyle-Font-Bold="true" DataKeyNames="Code">
                        <Columns>
                            <dnnweb:DnnGridBoundColumn HeaderText="grdCulturesCode" DataField="Code"></dnnweb:DnnGridBoundColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="grdCulturesText" DataField="Text"></dnnweb:DnnGridBoundColumn>
                            <dnnweb:DnnGridTemplateColumn HeaderText="grdCulturesActivateDeactivate">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" resourcekey="btnActivate" CommandName="Activate" ToolTip='<%# GetTooltipMessage((string)Eval("Code"), (bool)Eval("AllowStateChange")) %>' Visible='<%# !(bool)Eval("Active") %>' Enabled='<%# (bool)Eval("AllowStateChange") %>'></asp:LinkButton>
                                    <asp:LinkButton runat="server" resourcekey="btnDeactivate" Text="Deactivate" CommandName="Deactivate" ToolTip='<%# GetTooltipMessage((string)Eval("Code"), (bool)Eval("AllowStateChange")) %>'  Visible='<%# (bool)Eval("Active") %>' Enabled='<%# (bool)Eval("AllowStateChange") %>'></asp:LinkButton>
                                </ItemTemplate>
                            </dnnweb:DnnGridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </dnnweb:dnngrid>
            </div>
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton ID="hlkSave" runat="server" CssClass="dnnPrimaryAction" Text="Save" resourcekey="hlkSave" OnClick="hlkSave_Click"></asp:LinkButton></li>
        <li><asp:HyperLink ID="hlkCancel" runat="server" CssClass="dnnSecondaryAction" Text="Cancel" resourcekey="hlkCancel"></asp:HyperLink></li>
    </ul>
</div>


<script type="text/javascript">
    $(document).ready(function () {
        var setupModule = function () {
            jQuery('.scPortalConfiguration').dnnPanels(); 
        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // note that this will fire when _any_ UpdatePanel is triggered
            setupModule();
        });
    });

    function RenameZone() {
        var newName = "";
        while (newName == "") {
            newName = prompt("Enter the new name for the Virtual Database (VDB)", '<%= ddlZones.SelectedItem.Text %>');
            if (newName == null || newName == false)
                return false;
        }
        $("#<%= hfZoneName.ClientID %>").val(newName);
        return true;
    }

    function CreateZone() {
        var newName = "";
        while (newName == "") {
            newName = prompt("Enter the name for the new Virtual Database (VDB)", 'New VDB Portal <%= this.PortalSettings.PortalId %>');
            if (newName == null || newName == false)
                return false;
        }
        $("#<%= hfZoneName.ClientID %>").val(newName);
        return true;
    }
</script>