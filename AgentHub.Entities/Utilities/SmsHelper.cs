using System;
using System.Runtime.Remoting.Messaging;
using Twilio;

namespace AgentHub.Entities.Utilities
{
    public static class SmsHelper
    {
        private delegate void AsyncMethodCaller(SMSMessage message);

        public static void SendSMS(string toNumber, string message)
        {
            var caller = new AsyncMethodCaller(SendSmsMessageInSeperateThread);
            var callbackHandler = new AsyncCallback(AsyncCallback);

            var smsMessage = new SMSMessage {To = toNumber, Body = message};

            caller.BeginInvoke(smsMessage, callbackHandler, null);
        }

        private static void SendSmsMessageInSeperateThread(SMSMessage message)
        {
            var twilio = new TwilioRestClient(AppSettings.TwilioAccountSid, AppSettings.TwilioAuthToken);

            var twilioMessage = twilio.SendMessage(AppSettings.TwilioSmsAccountFrom, message.To, message.Body);

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
