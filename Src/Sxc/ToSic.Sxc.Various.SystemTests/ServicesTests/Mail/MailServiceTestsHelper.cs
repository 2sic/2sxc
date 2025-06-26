using System.Net.Mail;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Mail.Sys;

namespace ToSic.Sxc.ServicesTests.Mail;

public class MailServiceTestsHelper(IMailService mailSvc) 
{
    #region Helpers

    public const string Address = @"user@host";
    public const string DisplayName = @"Display Name";
    public const string Addresses = @"""John, Doe"" <user@host>, ""Bob, Smith"" <user2@host>";
    public const string Emails = @"a@a.com,b@b.com,c@c.com,d@d.com";

    public void MailAddressEqual(string input, string expMail, string expName)
    {
        // Actually tests System.Net.Mail.MailAddress
        // that is responsible for parsing input to email Address and display name
        var actual = ((MailServiceBase)mailSvc).MailAddress("test", input);
        Equal(expMail, actual.Address);
        Equal(expName, actual.DisplayName);
    }

    public void MailAddressEqual(string input, string expMail1, string expName1, string expMail2, string expName2)
    {
        // Actually tests System.Net.Mail.MailAddressCollection
        // that is responsible for parsing string input to list of MailAddresses
        var actual = new MailAddressCollection();

        ((MailServiceBase)mailSvc).AddMailAddresses("test", actual, input);
        Equal(expMail1, actual[0].Address);
        Equal(expName1, actual[0].DisplayName);
        Equal(expMail2, actual[1].Address);
        Equal(expName2, actual[1].DisplayName);
    }

    #endregion
}