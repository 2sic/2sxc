<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GettingStartedContent.ascx.cs" Inherits="ToSic.SexyContent.GettingStarted.GettingStartedContent" %>

<iframe id="frGettingStarted" src="http://gettingstarted.2sexycontent.org/AutoConfigure/Content.aspx" width="100%" height="300px"></iframe>

<script type="text/javascript">
    
    window.addEventListener("message", recieveMessage, false);

    function recieveMessage(event) {
        if (event.origin !== "http://gettingstarted.2sexycontent.org")
            return;

        // Data is sent as text because IE8 and 9 cannot send objects through postMessage
        var data = JSON.parse(event.data);

        if (data.uplink == "install") {
            if (confirm("Do you want to install this package? (" + data.package + ")")) {
                InstallPackage(data.package);
            }
        }
    }

    function InstallPackage(packageUrl) {

        var sf = $.ServicesFramework(<%= ModuleID %>);
        $.ajax({
            type: "POST",
            url: sf.getServiceRoot('ToSIC_SexyContent') + "GettingStarted/" + "InstallPackage",
            data: "package=" + encodeURIComponent(packageUrl),
            beforeSend: sf.setModuleHeaders
        }).done(function (e) {
            alert(e);
        }).fail(function (xhr, result, status) {
            alert("Something went wrong: " + status);
        });

    }

</script>

<%= LocalizeString("GetStarted.Text") %><br/><br/>
<asp:LinkButton runat="server" ID="btnInstallGettingStarted" ResourceKey="btnInstallGettingStarted" CssClass="dnnPrimaryAction"></asp:LinkButton>