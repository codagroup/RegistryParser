#region Usings
using NFluent;
using NUnit.Framework;
#endregion

namespace CODA.RegistryParser.Test;

[TestFixture]
internal class TestRegistryHive
{
    [Test]
    public void CheckHardAndSoftParsingErrors()
    {
        var sam = new RegistryHive($"{TestHelpers.HivePath}/SAM");
        sam.FlushRecordListsAfterParse = false;
        sam.ParseHive();

        Check.That(sam.SoftParsingErrors).IsEqualTo(0);
        Check.That(sam.HardParsingErrors).IsEqualTo(0);
    }
}