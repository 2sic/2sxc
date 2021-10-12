using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using ToSic.Eav;
using ToSic.Sxc.Web;
using MailMessage = System.Net.Mail.MailMessage;
using MailPriority = System.Net.Mail.MailPriority;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnMailService : MailService
    {
        public override string Send(MailMessage mailMessage)
        {
            var smtpServer = DotNetNuke.Entities.Host.Host.SMTPServer;
            if (string.IsNullOrEmpty(smtpServer)) return null; // DotNetNuke.Services.Localization.Localization.GetString("SMTPConfigurationProblem");

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    var strArray = smtpServer.Split(':');
                    smtpClient.Host = strArray[0];
                    smtpClient.Port = strArray.Length > 1 ? Convert.ToInt32(strArray[1]) : 25;
                    smtpClient.ServicePoint.MaxIdleTime = DotNetNuke.Entities.Host.Host.SMTPMaxIdleTime;
                    smtpClient.ServicePoint.ConnectionLimit = DotNetNuke.Entities.Host.Host.SMTPConnectionLimit;

                    var smtpAuthentication = DotNetNuke.Entities.Host.Host.SMTPAuthentication;
                    var smtpUsername = DotNetNuke.Entities.Host.Host.SMTPUsername;
                    var smtpPassword = DotNetNuke.Entities.Host.Host.SMTPPassword;

                    switch (smtpAuthentication)
                    {
                        case "1":
                            if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
                            {
                                smtpClient.UseDefaultCredentials = false;
                                smtpClient.Credentials = (ICredentialsByHost)new NetworkCredential(smtpUsername, smtpPassword);
                                break;
                            }
                            break;
                        case "2":
                            smtpClient.UseDefaultCredentials = true;
                            break;
                    }
                    smtpClient.EnableSsl = DotNetNuke.Entities.Host.Host.EnableSMTPSSL;
                    smtpClient.Send(mailMessage);
                    smtpClient.Dispose();
                    return "";
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException((Exception)ex);
                return string.Format(DotNetNuke.Services.Localization.Localization.GetString("FailedRecipient"), (object)ex.FailedRecipient);
            }
            catch (SmtpException ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException((Exception)ex);
                return DotNetNuke.Services.Localization.Localization.GetString("SMTPConfigurationProblem");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex.InnerException);
                    return ex.Message + Environment.NewLine + ex.InnerException.Message;
                    
                }
                else
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                    return ex.Message;
                }
            }
            finally
            {
                mailMessage.Dispose();
            }
        }

        public override string Send(
            string noParamOrder = Parameters.Protector,
            string mailFrom = null,
            string mailTo = null,
            string cc = null,
            string bcc = null,
            string replyTo = null,
            MailPriority priority = MailPriority.Normal,
            string subject = null,
            bool isBodyHtml = true,
            Encoding bodyEncoding = null,
            string body = null,
            List<Attachment> attachments = null)
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Send)}", $"{nameof(mailTo)},{nameof(body)}");

            return Mail.SendMail(
                mailFrom: mailFrom,
                mailTo: mailTo,
                cc: cc,
                bcc: bcc,
                replyTo: replyTo,
                priority: (DotNetNuke.Services.Mail.MailPriority)priority,
                subject: subject,
                bodyFormat: isBodyHtml ? DotNetNuke.Services.Mail.MailFormat.Html : DotNetNuke.Services.Mail.MailFormat.Text,
                bodyEncoding: bodyEncoding ?? Encoding.UTF8,
                body: body,
                attachments: attachments,
                smtpServer: "",
                smtpAuthentication: "",
                smtpUsername: "",
                smtpPassword: "",
                smtpEnableSSL: DotNetNuke.Entities.Host.Host.EnableSMTPSSL
                );
        }
    }
}
