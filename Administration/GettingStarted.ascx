<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GettingStarted.ascx.cs" Inherits="ToSic.SexyContent.Administration.GettingStarted" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>
<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

<iframe class="sc-iframe-gettingstarted" runat="server" id="ifrGettingStarted" src="http://gettingstarted.2sxc.org" style="border:none; width:100%; height:500px"></iframe>

<script type="text/javascript">
    $(document).ready(function () {
        $(window).bind("resize", function () {
            var GettingStartedFrame = $(".sc-iframe-gettingstarted");
            GettingStartedFrame.height($(window).height() - 50);
        });

        $(window).trigger("resize");
    });
</script>