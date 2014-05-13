<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm scSettings dnnClear">
    <%--<h2 class="dnnFormSectionHead"><a href="#"><asp:Label runat="server" ID="lblDataSourceConfiguration" ResourceKey="lblDataSourceConfiguration"></asp:Label></a></h2>
    <fieldset>
        <div style="margin-left:280px; margin-bottom:30px;">If this module should show something additinal / other than the data entered by the editor, you can configure this with the DataSource.</div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblDataSource" runat="server" ControlName="ddlDataSource" Suffix=":"></dnn:Label>
            <div>
                <asp:DropDownList runat="server" ID="ddlDataSource">
                    <asp:ListItem Text="Current module data" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Other datasource" Value="2"></asp:ListItem>
                </asp:DropDownList>
                <a href='<%= DotNetNuke.Common.Globals.NavigateURL(this.TabId, "editdatasource", "mid=" + this.ModuleId.ToString()) %>' id="hlkEditDataSource">Edit</a>
                <div style="margin-left:280px; margin-bottom:30px;">The <i>Current Module Data</i> is the default data source, delivering data configured by the editor / author.</div>
            </div>
        </div>
    </fieldset>--%>
    <h2 class="dnnFormSectionHead"><a href="#"><asp:Label runat="server" ID="lblDataSourcePublishing" ResourceKey="lblDataSourcePublishing"></asp:Label></a></h2>
    <fieldset>
        <div style="margin-left:280px; margin-bottom:30px;"><%= LocalizeString("lblDataSourcePublishing.LongText") %></div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblPublishSource" runat="server" ControlName="chkPublishSource" Suffix=":"></dnn:Label>
            <asp:CheckBox runat="server" ID="chkPublishSource" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblPublishStreams" runat="server" ControlName="txtPublishStreams" Suffix=":"></dnn:Label>
            <asp:TextBox runat="server" ID="txtPublishStreams" Text="Default,ListContent"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblTest" runat="server" Suffix=":"></dnn:Label>
            <a target="_blank" href="<%= GetJsonUrl() %>"><%= GetJsonUrl() %></a>
        </div>
    </fieldset>
</div>

<%--<script type="text/javascript">
    $(document).ready(function() {
        $("#hlkEditDataSource").click(function (e) {
            e.preventDefault();
            var url = $(this).attr("href");
            url += (url.indexOf("?") == -1 ? "?" : "&") + "datasourceid=" + $("#<%= ddlDataSource.ClientID %>").val() + "&popUp=true";
            window.location = url;
        });
    });
</script>--%>