<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Export.ascx.cs" Inherits="ToSic.SexyContent.Export" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel runat="server" class="dnnForm dnnSexyContentExport dnnClear" id="pnlChoose">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelExport"><asp:Label runat="server" ID="lblExportHeading" ResourceKey="lblExportHeading"></asp:Label></h2>
    <div class="dnnFormMessage dnnFormInfo">Many features are not automated yet – for example you can add the images used by your entities to your zip, and they will be imported automatically. To discover these features, please unpack the Getting-Started package. </div>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSelectContentTypes" runat="server" ControlName="grdTemplates" Suffix=":"></dnn:Label><br/>
                <dnnweb:dnngrid ID="grdContentTypes" runat="server" AutoGenerateColumns="false" EnableViewState="true" AllowMultiRowSelection="true">
                    <ClientSettings>
                        <Selecting AllowRowSelect="true" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="AttributeSetID" AllowSorting="True" HeaderStyle-Font-Bold="true">
                        <Columns>
                            <dnnweb:DnnGridClientSelectColumn HeaderText="Select">
                                <ItemStyle Width="30px" />
                            </dnnweb:DnnGridClientSelectColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="ID" DataField="AttributeSetID">
                                <ItemStyle Width="50px" />
                            </dnnweb:DnnGridBoundColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="Name" DataField="Name"></dnnweb:DnnGridBoundColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="Description" DataField="Description"></dnnweb:DnnGridBoundColumn>
                        </Columns>
                        <NoRecordsTemplate>
                            <asp:Label ID="lblNoRecords" runat="server" resourcekey="lblNoRecords"></asp:Label>
                        </NoRecordsTemplate>
                    </MasterTableView>
                </dnnweb:dnngrid>
        </div><br />
        <div class="dnnFormItem">
            <dnn:Label ID="lblSelectData" runat="server" ControlName="grdData" Suffix=":"></dnn:Label><br/>
                <dnnweb:dnngrid ID="grdData" runat="server" AutoGenerateColumns="false" EnableViewState="true" AllowMultiRowSelection="true">
                    <ClientSettings>
                        <Selecting AllowRowSelect="true" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="ID" AllowSorting="True" HeaderStyle-Font-Bold="true">
                        <Columns>
                            <dnnweb:DnnGridClientSelectColumn HeaderText="Select">
                                <ItemStyle Width="30px" />
                            </dnnweb:DnnGridClientSelectColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="ID" DataField="ID">
                                <ItemStyle Width="50px" />
                            </dnnweb:DnnGridBoundColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="Title" DataField="Title"></dnnweb:DnnGridBoundColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="ContentTypeID" DataField="ContentTypeID"></dnnweb:DnnGridBoundColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="ContentTypeName" DataField="ContentTypeName"></dnnweb:DnnGridBoundColumn>
                        </Columns>
                        <NoRecordsTemplate>
                            <asp:Label ID="lblNoRecords" runat="server" resourcekey="lblNoRecords"></asp:Label>
                        </NoRecordsTemplate>
                    </MasterTableView>
                </dnnweb:dnngrid>
        </div><br/>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSelectTemplates" runat="server" ControlName="grdTemplates" Suffix=":"></dnn:Label><br/>
                <dnnweb:dnngrid ID="grdTemplates" runat="server" AutoGenerateColumns="false" EnableViewState="true" AllowMultiRowSelection="true">
                    <ClientSettings>
                        <Selecting AllowRowSelect="true" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="TemplateID" AllowSorting="True" HeaderStyle-Font-Bold="true">
                        <Columns>
                            <dnnweb:DnnGridClientSelectColumn HeaderText="Select">
                                <ItemStyle Width="30px" />
                            </dnnweb:DnnGridClientSelectColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="ID" DataField="TemplateID">
                                <ItemStyle Width="50px" />
                            </dnnweb:DnnGridBoundColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="Name" DataField="Name"></dnnweb:DnnGridBoundColumn>
                            <dnnweb:DnnGridBoundColumn HeaderText="DemoEntity" DataField="DemoEntityID"></dnnweb:DnnGridBoundColumn>
                        </Columns>
                        <NoRecordsTemplate>
                            <asp:Label ID="lblNoRecords" runat="server" resourcekey="lblNoRecords"></asp:Label>
                        </NoRecordsTemplate>
                    </MasterTableView>
                </dnnweb:dnngrid>
        </div>
        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton ID="btnExport" runat="server" CssClass="dnnPrimaryAction" ResourceKey="hlkExport" OnClick="btnExport_Click"></asp:LinkButton></li>
            <li><asp:HyperLink ID="hlkCancel" runat="server" CssClass="dnnSecondaryAction" ResourceKey="hlkCancel"></asp:HyperLink></li>
        </ul>
    </fieldset>
</asp:Panel>
<asp:Panel runat="server" class="dnnForm dnnSexyContentExport dnnClear" id="pnlExportReport" Visible="false">
    <h2 class="dnnFormSectionHead" runat="server"><asp:Label runat="server" ID="lblExportReportHeading" ResourceKey="lblExportReportHeading"></asp:Label></h2>
    <asp:Label runat="server" ID="lblContentTypeCount"></asp:Label>
</asp:Panel>