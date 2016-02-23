<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GettingStartedFrame.ascx.cs" Inherits="ToSic.SexyContent.GettingStarted.GettingStartedFrame" %>

<%--<div style="position:relative;">
    <iframe id="frGettingStarted" src="<%= GettingStartedUrl() %>" width="100%" height="300px"></iframe>

    <div class="sc-loading" id="pnlLoading" style="display:none;">
        <i class="icon-sxc-spinner animate-spin"></i>
        <br/>
        <br/>
        <span class="sc-loading-label">
            installing <span id="packageName">.</span>
        </span>
    </div>

    <script type="text/javascript">

        window.addEventListener("message", forwardMessage<%= ModuleID %>, false);

        function forwardMessage<%= ModuleID %>(event) {
            var modId = <%= ModuleID %>;
            processInstallMessage(event, modId);
        }

    </script>
    

</div>--%>