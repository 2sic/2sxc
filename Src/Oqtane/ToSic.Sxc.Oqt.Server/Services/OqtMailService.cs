using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Oqt.Server.Services;

internal class OqtMailService : MailServiceBase
{
    private readonly LazySvc<ISettingRepository> _settingRepositoryLazy;

    public OqtMailService(LazySvc<ISettingRepository> settingRepositoryLazy, LazySvc<IUser> userLazy) : base(userLazy)
    {
        ConnectLogs([
            _settingRepositoryLazy = settingRepositoryLazy
        ]);
    }

    protected override SmtpClient SmtpClient()
    {
        var settings = GetSettings();
        if (!settings.ContainsKey("SMTPHost") || settings["SMTPHost"] == "" 
                                              || !settings.ContainsKey("SMTPPort") || settings["SMTPPort"] == "" 
                                              || !settings.ContainsKey("SMTPSSL") || settings["SMTPSSL"] == "" 
                                              || !settings.ContainsKey("SMTPSender") || settings["SMTPSender"] == "") 
            throw new ConfigurationErrorsException("SMTP configuration problem. Some settings are missing. Please configure 'SMTP Settings' in 'Site Settings'.");

        try
        {
            // construct SMTP Client
            var client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = settings["SMTPHost"],
                Port = int.Parse(settings["SMTPPort"]),
                EnableSsl = bool.Parse(settings["SMTPSSL"])
            };

            if (settings["SMTPUsername"] != "" && settings["SMTPPassword"] != "")
            {
                client.Credentials = new NetworkCredential(settings["SMTPUsername"], settings["SMTPPassword"]);
            }

            return client;
        }
        catch (Exception ex)
        {
            throw new ConfigurationErrorsException("SMTP configuration problem.", ex);
        }
    }

    private Dictionary<string, string> GetSettings()
    {
        var settings = _settingRepositoryLazy.Value.GetSettings(EntityNames.Site, App.Site.Id).ToList();
        return settings.OrderBy(item => item.SettingName).ToList().ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);
    }
}