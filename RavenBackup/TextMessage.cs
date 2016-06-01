using Twilio;

namespace RavenBackup
{
    public static class TextMessage
    {
        public static void SendMessage(string message)
        {
            if(TwilioSection.Enabled)
            {
                var twilio = new TwilioRestClient(TwilioSection.AccountSID, TwilioSection.AuthToken);
                twilio.SendMessage(TwilioSection.FromNumber, TwilioSection.ToNumber, message);
            }
        }
    }
}