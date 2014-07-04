<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditContentGroup.ascx.cs" Inherits="ToSic.SexyContent.EditContentGroup" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Controls/ItemForm.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Styles/Edit.css"></dnn:DnnCssInclude>
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Controls/ItemForm.js" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Controls/ItemFormEntityModelCreator.js" Priority="200" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/ItemForm.js" Priority="300" />

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular-ui-tree.min.css" Priority="60" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular.min.js" Priority="60" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular-ui-tree.min.js" Priority="61" />

<!-- Dimension-Navigation -->
<asp:Panel runat="server" CssClass="dnnForm dimensionTabs" ID="pnlDimensionNav">
    <ul class="dnnAdminTabNav dnnClear ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all">
        <asp:Repeater runat="server" ID="rptDimensions" OnItemCommand="rptDimensions_ItemCommand">
            <ItemTemplate>
                <li class='<%# "ui-state-default ui-corner-top" + (LanguageID.ToString() == Eval("DimensionID").ToString() ? " ui-tabs-selected ui-state-active ui-tabs-active" : "") + (DefaultLanguageID == (int)Eval("DimensionID") ? " sc-default-language" : "") %>'>
                    <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# Eval("Name") %>' OnClientClick='<%# "if(!ChangeLanguage(" + LanguageID.ToString() + ", " + Eval("DimensionID").ToString() + ", \"" + GetCultureUrl((int)Eval("DimensionID")) + "\" )) return false;"  %>'
                        CommandName="ChangeLanguage" CommandArgument='<%# Eval("DimensionID") %>'></asp:LinkButton>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <asp:HiddenField runat="server" ID="hfMustSave" Value="false" />
</asp:Panel>
<!-- / Dimension-Navigation -->

<div class="dnnForm scNewOrEditItem dnnClear">
    <asp:Panel runat="server" id="pnlLanguageNotActive" Visible="false">
        <br />
        <div class="dnnFormMessage dnnFormInfo">
            The language you are trying to edit (<i><asp:Literal runat="server" ID="litLanguageName"></asp:Literal></i>) is not active in 2SexyContent Portal Configuration.
            <asp:LinkButton runat="server" Visible="False" ID="btnActivateLanguage" Text="Activate Language" OnClick="btnActivateLanguage_Click"></asp:LinkButton>
        </div>
        <br />
    </asp:Panel>
    <asp:PlaceHolder runat="server" ID="phNewOrEditControls"></asp:PlaceHolder>
    <asp:Panel runat="server" ID="pnlActions" Visible="true">
        <div class="sc-field-error dnnFormWarning dnnFormMessage" style="display:none;"><%= LocalizeString("FieldValidationFailed.Text") %></div>
        <ul class="dnnActions dnnClear">
            <li style="line-height: 32px; margin-right: 20px;"><asp:CheckBox runat="server" ID="chkPublished"/><%= LocalizeString("Published.Text") %> </li>
            <li><asp:LinkButton ID="btnUpdate" resourcekey="btnUpdate" runat="server" CommandName="Update" OnClick="btnUpdate_Click" CssClass="dnnPrimaryAction eav-save" /></li>
            <li><asp:LinkButton ID="btnCancel" resourcekey="btnCancel" runat="server" CommandName="Cancel" CausesValidation="false" onclick="btnCancel_Click" CssClass="dnnSecondaryAction" /></li>
            <li><a style="min-width:25px;" class="dnnSecondaryAction sc-action-trigger-others">...</a></li>
            <li><asp:LinkButton ID="btnDelete" visible="false" runat="server" CommandName="Delete" CausesValidation="false" OnClick="btnDelete_Click" CssClass="dnnSecondaryAction sc-action-other" /></li>
            <li><asp:Hyperlink CssClass="dnnSecondaryAction hlkChangeContent sc-action-other" runat="server" ID="hlkChangeContent" ResourceKey="hlkChangeContent" Visible="False"></asp:Hyperlink></li>
        </ul>
    </asp:Panel>
</div>

<script type="text/javascript">
    $(document).ready(function () {
        var setupModule = function () {
            jQuery('.scNewOrEditItem').dnnPanels();
        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // note that this will fire when _any_ UpdatePanel is triggered
            setupModule();
        });

        // Show message if there are field validation errors
        $(".eav-save").click(function () {
            $(".sc-field-error").toggle($(".dnnFormMessage.dnnFormError:visible").size() > 0);
        });

        // Show "Other" actions when ... is clicked
        $(".sc-action-trigger-others").click(function () {
            $(".sc-action-other").css("display", "inline-block");
            $(this).hide();
        });
    });

    function ChangeLanguage(CurrentLanguage, NewLanguage, NewLanguageUrl) {
        if (CurrentLanguage == NewLanguage)
            return false;

        var mustSave = false;
        $(".eav-form").each(function(i,e) {
            if(e.Controller.IsChanged())
                mustSave = true;
        });
        $("input[type=hidden][id$='_hfReferenceChanged']").each(function() {
            if($(this).val() == 'true')
                mustSave = true;
        });

        $("input[type=hidden][id$='_hfMustSave']").val(mustSave);

        if (mustSave)
            return confirm("<%= LocalizeString("ChangeLanguageConfirm.Text") %>");
        else {
            window.location = NewLanguageUrl;
            return false;
        }
    }
</script>