using System.Configuration;
using System.Net;
using System.Net.Mail;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using ToSic.Sxc.Services.Mail.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Dnn.Services;

internal class DnnMailService(
    LazySvc<IUser> userLazy,
    IMailSettings mailSettings,
    IHostSettingsService hostSettingsService,
    IPortalController portalController) : MailServiceBase(userLazy)
{
    protected override SmtpClient SmtpClient()
    {
        var portalId = PortalSettings.Current?.PortalId ?? Null.NullInteger;
        var smtpServer = mailSettings.GetServer(portalId);
        if (string.IsNullOrEmpty(smtpServer)) 
            throw new ConfigurationErrorsException(DotNetNuke.Services.Localization.Localization.GetString("SMTPConfigurationProblem"));

        try
        {
            var client = new SmtpClient();

            var strArray = smtpServer.Split(':');
            client.Host = strArray[0];
            client.Port = strArray.Length > 1 ? Convert.ToInt32(strArray[1]) : 25;
            client.ServicePoint.MaxIdleTime = checked((int)mailSettings.GetMaxIdleTime(portalId).TotalMilliseconds);
            client.ServicePoint.ConnectionLimit = mailSettings.GetConnectionLimit(portalId);

            var smtpAuthentication = mailSettings.GetAuthentication(portalId);
            var smtpUsername = mailSettings.GetUsername(portalId);
            var smtpPassword = mailSettings.GetPassword(portalId);

            switch (smtpAuthentication)
            {
                case "1":
                    if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = (ICredentialsByHost)new NetworkCredential(smtpUsername, smtpPassword);
                        break;
                    }
                    break;
                case "2":
                    client.UseDefaultCredentials = true;
                    break;
            }

            // DNN 10.1's IMailSettings implementation does not implement GetSecureConnectionEnabled.
            client.EnableSsl = mailSettings.IsPortalEnabled(portalId)
                ? PortalController.GetPortalSettingAsBoolean(portalController, "SMTPEnableSSL", portalId, false)
                : hostSettingsService.GetBoolean("SMTPEnableSSL", false);

            return client;
        }
        catch (Exception ex)
        {
            throw new ConfigurationErrorsException(DotNetNuke.Services.Localization.Localization.GetString("SMTPConfigurationProblem"), ex);
        }
    }
}
