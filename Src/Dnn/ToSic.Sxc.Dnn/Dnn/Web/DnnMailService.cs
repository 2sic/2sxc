using System;
using System.Net;
using System.Net.Mail;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnMailService : MailServiceBase
    {
        protected override SmtpClient SmtpClient()
        {
            var smtpServer = DotNetNuke.Entities.Host.Host.SMTPServer;
            if (string.IsNullOrEmpty(smtpServer)) return null; // DotNetNuke.Services.Localization.Localization.GetString("SMTPConfigurationProblem");

            var client = new SmtpClient();

            var strArray = smtpServer.Split(':');
            client.Host = strArray[0];
            client.Port = strArray.Length > 1 ? Convert.ToInt32(strArray[1]) : 25;
            client.ServicePoint.MaxIdleTime = DotNetNuke.Entities.Host.Host.SMTPMaxIdleTime;
            client.ServicePoint.ConnectionLimit = DotNetNuke.Entities.Host.Host.SMTPConnectionLimit;

            var smtpAuthentication = DotNetNuke.Entities.Host.Host.SMTPAuthentication;
            var smtpUsername = DotNetNuke.Entities.Host.Host.SMTPUsername;
            var smtpPassword = DotNetNuke.Entities.Host.Host.SMTPPassword;

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

            client.EnableSsl = DotNetNuke.Entities.Host.Host.EnableSMTPSSL;

            return client;
        }
    }
}
