using System;
using System.Globalization;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using AgentHub.Entities.Utilities;
using Microsoft.AspNet.Identity;

namespace AgentHub.Web.Identity
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            EmailHelper.SendEmail(message.Destination, message.Subject, message.Body, isBodyHtml: true);

            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            SmsHelper.SendSMS(message.Destination, message.Body);

            return Task.FromResult(0);
        }
    }
}