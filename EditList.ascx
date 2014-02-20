<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.EditList" Codebehind="EditList.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<div class="dnnForm dnnSexyContentEditList dnnClear">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelSexyContentEditList">
        <asp:Label runat="server" ID="lblEditListHeading" ResourceKey="lblEditListHeading"></asp:Label>
    </h2>
    <asp:Panel ID="pnlEditListHeader" runat="server" Visible="false">
        <asp:LinkButton runat="server" ID="btnEditListHeader" OnClick="btnEditListHeader_Click" resourcekey="btnEditListHeader"></asp:LinkButton>
        <asp:Label runat="server" ID="lblEditListHeader" resourcekey="lblEditListHeader"></asp:Label>
    </asp:Panel>
    <fieldset>
        <dnnweb:dnngrid CssClass="GridEntities" ID="grdEntities" runat="server" AutoGenerateColumns="false" EnableViewState="true" OnItemCommand="grdEntities_ItemCommand" OnRowDrop="grdEntities_RowDrop" OnNeedDatasource="grdEntities_NeedDatasource">
            <ClientSettings AllowRowsDragDrop="true" AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="false"></Selecting>
            </ClientSettings>
            <MasterTableView DataKeyNames="ID" HeaderStyle-Font-Bold="true">
                <Columns>
                    <telerik:GridDragDropColumn HeaderStyle-Width="18px" ItemStyle-Width="18px" DragImageUrl="~/DesktopModules/ToSIC_SexyContent/Images/Drag.gif" />
                    <dnnweb:DnnGridTemplateColumn HeaderText="Title">
                        <ItemTemplate>
                            <asp:HyperLink CssClass="sc-list-itemedit" ID="hlkEdit" runat="server" NavigateUrl='<%# GetEditUrl((int)Eval("ID")) %>' Text='<%# String.IsNullOrEmpty(Eval("EntityTitle").ToString()) ? "..." : Eval("EntityTitle") %>'></asp:HyperLink>
                        </ItemTemplate>
                    </dnnweb:DnnGridTemplateColumn>
                    <dnnweb:DnnGridBoundColumn HeaderText="ID" DataField="EntityID" HeaderStyle-Width="100px" ItemStyle-Width="100px"></dnnweb:DnnGridBoundColumn>
                    <dnnweb:DnnGridButtonColumn UniqueName="AddColumn" ButtonType="ImageButton" ImageUrl="~/Images/Add.gif" CommandName="add" HeaderStyle-Width="25px" ItemStyle-Width="25px"></dnnweb:DnnGridButtonColumn>
                    <dnnweb:DnnGridButtonColumn UniqueName="AddWithEditColumn" ButtonType="ImageButton" ImageUrl="~/Images/Add.gif" CommandName="addwithedit" HeaderStyle-Width="25px" ItemStyle-Width="25px"></dnnweb:DnnGridButtonColumn>
                </Columns>
                <NoRecordsTemplate>
                    <asp:Label ID="lblNoRecords" runat="server" resourcekey="lblNoRecords"></asp:Label>
                </NoRecordsTemplate>
            </MasterTableView>
        </dnnweb:dnngrid>
        <ul class="dnnActions dnnClear">
            <li><asp:HyperLink ID="hlkCancel" runat="server" CssClass="dnnPrimaryAction" ResourceKey="hlkCancel"></asp:HyperLink></li>
        </ul>
    </fieldset>
</div>

<style type="text/css">
    .rgDataDiv { height: auto !important; }
    .RadGrid .rgRow td, .RadGrid .rgAltRow td, .RadGrid .rgEditRow td, .RadGrid .rgFooter td { padding: 5px 7px 5px 7px; vertical-align: middle; }
    .RadGrid .rgHeader, .RadGrid th.rgResizeCol { font-weight: normal; padding: 4px 7px 3px 7px; text-align: left; line-height:normal; }
    .RadGrid_Default .rgRow td, .RadGrid_Default .rgAltRow td, .RadGrid_Default .rgEditRow td, .RadGrid_Default .rgFooter td { border:none; }
    .GridItemDropIndicator_Default { z-index: 3005 !important; }

</style>

<script type="text/javascript">
    $(document).ready(function () {
        // Disable jScrollPane in DNN 7 because Drag&Drop does not work when enabled
        if ($.fn.jScrollPane)
            $.fn.jScrollPane = function() {};
    });
</script>