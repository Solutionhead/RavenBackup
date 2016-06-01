using System;
using NUnit.Framework;
using RavenBackup;

namespace Tests
{
    [TestFixture]
    public class RegexTests
    {
        [Test]
        public void Matches_filenames_as_expected()
        {
            DateTime timestamp;
            Assert.IsTrue(RegexHelper.MatchFilename("20000101 010203.raven", out timestamp));
            Assert.AreEqual(new DateTime(2000, 1, 1, 1, 2, 3, DateTimeKind.Utc), timestamp);

            Assert.IsTrue(RegexHelper.MatchFilename("20000101010203.raven", out timestamp));
            Assert.AreEqual(new DateTime(2000, 1, 1, 1, 2, 3, DateTimeKind.Utc), timestamp);

            Assert.IsTrue(RegexHelper.MatchFilename("20000101   010203.raven", out timestamp));
            Assert.AreEqual(new DateTime(2000, 1, 1, 1, 2, 3, DateTimeKind.Utc), timestamp);

            Assert.IsFalse(RegexHelper.MatchFilename("20000101 010203.txt", out timestamp));
            Assert.IsFalse(RegexHelper.MatchFilename("20000101 10203.raven", out timestamp));
        }
    }
}
