using System.Configuration;

namespace RavenBackup
{
    public static class Configuration
    {
        public static string DestinationPath  { get { return ConfigurationManager.AppSettings["destinationPath"]; } }
        public static string RavenUrl { get { return ConfigurationManager.AppSettings["ravenUrl"]; } }
    }
}