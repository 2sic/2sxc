<%@ Control Language="C#" Inherits="ToSic.Eav.ManagementUI.Entity_EditCustom" Codebehind="Entity_Edit.ascx.cs" AutoEventWireup="True" %>
<%@ Register src="../Controls/DimensionMenu.ascx" tagPrefix="Eav" tagName="DimensionMenu" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<dnn:Label ID="FieldLabel" runat="server" Suffix=":" />
<Eav:DimensionMenu ID="DimensionMenu1" runat="server" />

<div style="overflow:hidden; margin-bottom: 13px">
    <div class="eav-field-control dnnLeft">
	    <asp:DropDownList ID="DropDownList1" runat="server" DataTextField="Text" DataValueField="Value" AppendDataBoundItems="True" OnDataBound="DropDownList1_DataBound" CssClass="dnnFormInput eav-entity-selector">
		    <Items>
			    <asp:ListItem Text="(none)" Value="" />
		    </Items>
	    </asp:DropDownList>
	    <asp:HiddenField ID="hfEntityIds" runat="server" Visible="False" />
	    <asp:Panel runat="server" ID="pnlMultiValues" CssClass="MultiValuesWrapper" Visible="False"/>
	    <asp:PlaceHolder runat="server" ID="phAddMultiValue" Visible="False">
		    <a href="javascript:void(0)" class="AddValue dnnSecondaryAction">Add Value</a>
	    </asp:PlaceHolder>
    </div>
</div>