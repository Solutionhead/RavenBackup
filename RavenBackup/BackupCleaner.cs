using System;
using System.IO;
using System.Linq;

namespace RavenBackup
{
    public static class BackupCleaner
    {
        public static void CleanupBackups(DateTime utcNow, params string[] databaseNames)
        {
            foreach(var databaseName in databaseNames)
            {
                var directoryInfo = new DirectoryInfo(Path.Combine(Configuration.DestinationPath, databaseName));
                if(!directoryInfo.Exists)
                {
                    return;
                }

                foreach(var file in directoryInfo.GetFiles("*.raven", SearchOption.TopDirectoryOnly)
                    .Where(f =>
                        {
                            DateTime timestamp;
                            return RegexHelper.MatchFilename(f.Name, out timestamp) && (utcNow - timestamp).TotalDays > Configuration.BackupExpirationDays;
                        }))
                {
                    file.Delete();
                }
            }
        }
    }
}