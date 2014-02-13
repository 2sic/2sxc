<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Registers.ascx.cs" Inherits="ToSic.SexyContent.Administration.Registers" %>

<asp:Panel runat="server" CssClass="dnnForm administrationRegistersTab" ID="pnlAdministrationRegisters">
    <ul class="dnnAdminTabNav dnnClear ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all">
        <asp:Repeater runat="server" ID="rptRegisters">
            <ItemTemplate>
                <li class='<%# "ui-state-default ui-corner-top" + ((bool)Eval("Active") ? " ui-tabs-selected ui-state-active ui-tabs-active" : "") %>'>
                    <a href="<%# Eval("Url") %>"><%# Eval("Name") %></a>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <asp:HiddenField runat="server" ID="hfMustSave" Value="false" />
</asp:Panel>