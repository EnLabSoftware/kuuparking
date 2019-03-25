using System;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;

namespace AgentHub.Entities.Utilities
{
    public  static class EmailHelper
    {
        private delegate void AsyncMethodCaller(MailMessage message);

        public static void SendEmail(string toAddress, string subject, string body, bool isBodyHtml = false, string ccAddress = "", string bCCAddress = "")
        {
            var caller = new AsyncMethodCaller(SendMailInSeperateThread);
            var callbackHandler = new AsyncCallback(AsyncCallback);

            var message = new MailMessage
            {
                From = new MailAddress(AppSettings.WebMasterEmailAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };
            message.To.Add(toAddress);
            if (!string.IsNullOrEmpty(ccAddress))
                message.CC.Add(ccAddress);
            if (!string.IsNullOrEmpty(bCCAddress))
                message.Bcc.Add(bCCAddress);

            caller.BeginInvoke(message, callbackHandler, null);
        }

        private static void SendMailInSeperateThread(MailMessage message)
        {
            try
            {
                var smtpServer = new SmtpClient(AppSettings.SmtpServer)
                {
                    Port = AppSettings.SmtpPort.ToInt(),
                    Credentials =
                        new System.Net.NetworkCredential(AppSettings.WebMasterEmailAddress,
                            AppSettings.WebMasterEmailPassword),
                    EnableSsl = AppSettings.SmtpEnableSsl.ToBool()
                };

                smtpServer.SendAsync(message, string.Empty);
            }
            catch (Exception exception)
            {
                // This is very necessary to catch errors since we are in
                // a different context & thread
                LogHelper.LogException(exception);
            }
        }

        private static void AsyncCallback(IAsyncResult ar)
        {
            try
            {
                var result = (AsyncResult)ar;
                var caller = (AsyncMethodCaller)result.AsyncDelegate;
                caller.EndInvoke(ar);
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
            }
        }
    }
}
