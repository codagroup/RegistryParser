#region Usings
using NFluent;
using NUnit.Framework;
using CODA.RegistryParser.Cells;
using System.Security.Cryptography.X509Certificates;
#endregion

namespace CODA.RegistryParser.Test;

[TestFixture]
internal class TestSkCellRecord
{
    [Test]
    public void SkRecordxAclNoDataForAceRecordsInSacl()
    {
        var ntUserSlack = new RegistryHive($"{TestHelpers.HivePath}/NTUSER slack.DAT");
        ntUserSlack.FlushRecordListsAfterParse = false;
        ntUserSlack.ParseHive();

        var sk = ntUserSlack.CellRecords[0x80] as SkCellRecord;

        Check.That(sk).IsNotNull();
        if (sk is not null)
        {
            Check.That(sk.SecurityDescriptor).IsNotNull();
            if (sk.SecurityDescriptor is not null)
            {
                Check.That(sk.SecurityDescriptor.Dacl).IsNotNull();
                if (sk.SecurityDescriptor.Dacl is not null)
                {
                    Check.That(sk.SecurityDescriptor.Dacl.AceRecords).IsNotNull();
                    Check.That(sk.SecurityDescriptor.Dacl.AceRecords.Count).IsEqualTo(sk.SecurityDescriptor.Dacl.AceCount);
                    Check.That(sk.SecurityDescriptor.Dacl.AceRecords.ToString()).IsNotEmpty();
                }
                Check.That(sk.SecurityDescriptor.Sacl).IsNotNull();
                if (sk.SecurityDescriptor.Sacl is not null)
                {
                    Check.That(sk.SecurityDescriptor.Sacl.AceRecords).IsNotNull();
                    Check.That(sk.SecurityDescriptor.Sacl.AceRecords.Count).IsEqualTo(0);
                    // this is a strange case where there is no data to build ace records
                    Check.That(sk.SecurityDescriptor.Sacl.AceRecords.ToString()).IsNotEmpty();
                }
            }
            Check.That(sk.ToString()).IsNotEmpty();
        }
    }

    [Test]
    public void VerifySkInfo()
    {
        var sam = new RegistryHive($"{TestHelpers.HivePath}/SAM");
        sam.FlushRecordListsAfterParse = false;
        sam.ParseHive();

        var key = sam.GetKey(@"SAM\Domains\Account");

        Check.That(key).IsNotNull();
        if (key is not null)
        {
            var sk = sam.CellRecords[key.NkRecord.SecurityCellIndex] as SkCellRecord;

            Check.That(sk).IsNotNull();
            if (sk is not null)
            {
                Check.That(sk.ToString()).IsNotEmpty();
                Check.That(sk.Size).IsStrictlyGreaterThan(0);
                Check.That(sk.Reserved).IsInstanceOf<ushort>();

                Check.That(sk.DescriptorLength).IsStrictlyGreaterThan(0);
            }
        }
    }
}