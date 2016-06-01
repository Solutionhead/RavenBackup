using System.Configuration;
using System.Reflection;

namespace RavenBackup
{
    public class TwilioSection : ConfigurationSection
    {
        public static bool Enabled { get { return Section._Enabled; } set { Section._Enabled = value; } }
        public static string AccountSID { get { return Section._AccountSID; } set { Section._AccountSID = value; } }
        public static string AuthToken { get { return Section._AuthToken; } set { Section._AuthToken = value; } }
        public static string FromNumber { get { return Section._FromNumber; } set { Section._FromNumber = value; } }
        public static string ToNumber { get { return Section._ToNumber; } set { Section._ToNumber = value; } }

        private const string EnabledName = "enabled";
        private const string AccountSIDName = "accountSID";
        private const string AuthTokenName = "authToken";
        private const string FromNumberName = "fromNumber";
        private const string ToNumberName = "toNumber";

        private static TwilioSection Section
        {
            get { return _section ?? (_section = ConfigurationManager.GetSection(MethodBase.GetCurrentMethod().DeclaringType.Name) as TwilioSection ?? new TwilioSection()); }
        }
        private static TwilioSection _section;

        [ConfigurationProperty(EnabledName, IsRequired = true, DefaultValue = "False")]
        public bool _Enabled
        {
            get { return (bool)this[EnabledName]; }
            set { this[EnabledName] = value; }
        }

        [ConfigurationProperty(AccountSIDName, IsRequired = true, DefaultValue = "")]
        public string _AccountSID
        {
            get { return (string)this[AccountSIDName]; }
            set { this[AccountSIDName] = value; }
        }

        [ConfigurationProperty(AuthTokenName, IsRequired = true, DefaultValue = "")]
        public string _AuthToken
        {
            get { return (string)this[AuthTokenName]; }
            set { this[AuthTokenName] = value; }
        }

        [ConfigurationProperty(FromNumberName, IsRequired = true, DefaultValue = "")]
        public string _FromNumber
        {
            get { return (string)this[FromNumberName]; }
            set { this[FromNumberName] = value; }
        }

        [ConfigurationProperty(ToNumberName, IsRequired = true, DefaultValue = "")]
        public string _ToNumber
        {
            get { return (string)this[ToNumberName]; }
            set { this[ToNumberName] = value; }
        }
    }
}