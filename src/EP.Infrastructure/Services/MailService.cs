using EP.Application.Common.Interfaces.Services;
using EP.Application.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices.JavaScript;

namespace EP.Infrastructure.Services
{
    public class MailService : IMailService
    {
        private readonly MailSetting _mailSetting;

        public MailService(IOptions<MailSetting> mailSetting)
        {
            _mailSetting = mailSetting.Value;
        }
        public void Send(string receiver, string subject, string content)
        {
            var to = new MailAddress(receiver);
            var from = new MailAddress(_mailSetting.Sender);
            using (var email = new MailMessage(from, to))
            using (var smtp = new SmtpClient())
            {
                email.Subject = subject;
                email.Body = content;
                email.IsBodyHtml = true;

                smtp.Host = _mailSetting.Host;
                smtp.Port = _mailSetting.Port;
                smtp.Credentials = new NetworkCredential(_mailSetting.Sender, _mailSetting.Password);
                smtp.EnableSsl = true;

                smtp.Send(email);
            }
        }
    }
}
