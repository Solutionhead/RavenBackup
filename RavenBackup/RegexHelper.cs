using System;
using System.Text.RegularExpressions;

namespace RavenBackup
{
    public static class RegexHelper
    {
        public static bool MatchFilename(string text, out DateTime utcTimestamp)
        {
            utcTimestamp = new DateTime();
            var match = FilenameRegex.Match(text);
            if(match.Success)
            {
                utcTimestamp = new DateTime(
                    int.Parse(match.Groups["year"].Value),
                    int.Parse(match.Groups["month"].Value),
                    int.Parse(match.Groups["day"].Value),
                    int.Parse(match.Groups["hour"].Value),
                    int.Parse(match.Groups["minute"].Value),
                    int.Parse(match.Groups["second"].Value),
                    DateTimeKind.Utc);
                return true;
            }

            return false;
        }

        private static readonly Regex FilenameRegex = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})\s*(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})\.raven", RegexOptions.Compiled);
    }
}