#region Usings
using NFluent;
using NUnit.Framework;
using CODA.RegistryParser.Lists;
using CODA.RegistryParser.Other;
#endregion

namespace CODA.RegistryParser.Test;

[TestFixture]
internal class TestRegistryOther
{
    [Test]
    public void ExportToRegFormatNullKey()
    {
        Abstractions.RegistryKey? key = null;
        Check.ThatCode(() => { Helpers.ExportToReg(@"exportTest.reg", key!, HiveTypeEnum.Sam, true); })
            .Throws<NullReferenceException>();
    }

    [Test]
    public void ExportToRegFormatRecursive()
    {
        var samOnDemand = new RegistryHiveOnDemand($"{TestHelpers.HivePath}/SAM");
        var key = samOnDemand.GetKey(@"SAM\Domains\Account");

        var exported = Helpers.ExportToReg(@"exportTest.reg", key, HiveTypeEnum.Sam, true);

        Check.That(exported).IsTrue();
    }

    [Test]
    public void ExportToRegFormatSingleKey()
    {
        var samOnDemand = new RegistryHiveOnDemand($"{TestHelpers.HivePath}/SAM");
        var key = samOnDemand.GetKey(@"SAM\Domains\Account");

        var exported = Helpers.ExportToReg(@"exportSamTest.reg", key, HiveTypeEnum.Sam, false);

        Check.That(exported).IsTrue();

        var ntUser1OnDemand = new RegistryHiveOnDemand($"{TestHelpers.HivePath}/NTUSER1.DAT");
        key = ntUser1OnDemand.GetKey(@"CsiTool-CreateHive-{00000000-0000-0000-0000-000000000000}\Console");

        exported = Helpers.ExportToReg(@"exportntuser1Test.reg", key, HiveTypeEnum.NtUser, false);

        Check.That(exported).IsTrue();

        var security = new RegistryHiveOnDemand($"{TestHelpers.HivePath}/SECURITY");
        key =
            security.GetKey(
                @"CsiTool-CreateHive-{00000000-0000-0000-0000-000000000000}\Policy\Accounts\S-1-5-9");

        exported = Helpers.ExportToReg(@"exportsecTest.reg", key, HiveTypeEnum.Security, false);

        Check.That(exported).IsTrue();

        var systemOnDemand = new RegistryHiveOnDemand($"{TestHelpers.HivePath}/SYSTEM");
        key =
            systemOnDemand.GetKey(
                @"CsiTool-CreateHive-{00000000-0000-0000-0000-000000000000}\ControlSet001\Enum\ACPI\PNP0C02\1");

        exported = Helpers.ExportToReg(@"exportsysTest.reg", key, HiveTypeEnum.System, false);

        Check.That(exported).IsTrue();

        var usrClassFtp = new RegistryHiveOnDemand($"{TestHelpers.HivePath}/UsrClass FTP.dat");
        key = usrClassFtp.GetKey(@"S-1-5-21-2417227394-2575385136-2411922467-1105_Classes\.3g2");

        exported = Helpers.ExportToReg(@"exportusrTest.reg", key, HiveTypeEnum.UsrClass, false);

        Check.That(exported).IsTrue();

        var samDupeNameOnDemand = new RegistryHiveOnDemand($"{TestHelpers.HivePath}/SAM_DUPENAME");
        key = samDupeNameOnDemand.GetKey(@"SAM\SAM\Domains\Account\Aliases\000003E9");

        exported = Helpers.ExportToReg(@"exportotherTest.reg", key, HiveTypeEnum.Other, false);

        Check.That(exported).IsTrue();

        var usrclassDeleted = new RegistryHive($"{TestHelpers.HivePath}/UsrClassDeletedBags.dat");
        usrclassDeleted.RecoverDeleted = true;
        usrclassDeleted.FlushRecordListsAfterParse = false;
        usrclassDeleted.ParseHive();
        key =
            usrclassDeleted.GetKey(
                @"S-1-5-21-146151751-63468248-1215037915-1000_Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU\1");

        exported = Helpers.ExportToReg(@"exportDeletedTest.reg", key, HiveTypeEnum.UsrClass, false);

        Check.That(exported).IsTrue();
    }

    [Test]
    public void ExportUsrClassToCommonFormatWithDeleted()
    {
        var usrclassDeleted = new RegistryHive($"{TestHelpers.HivePath}/UsrClassDeletedBags.dat");
        usrclassDeleted.RecoverDeleted = true;
        usrclassDeleted.FlushRecordListsAfterParse = false;
        usrclassDeleted.ParseHive();

        usrclassDeleted.ExportDataToCommonFormat("UsrClassDeletedExport.txt", true);

        usrclassDeleted = new RegistryHive($"{TestHelpers.HivePath}/UsrClassDeletedBags.dat");
        usrclassDeleted.FlushRecordListsAfterParse = true;
        usrclassDeleted.ParseHive();

        usrclassDeleted.ExportDataToCommonFormat("UsrClassDeletedWithFlushExport.txt", true);
    }

    [Test]
    public void GetEnumFromDescriptionAndViceVersa()
    {
        var samOnDemand = new RegistryHiveOnDemand($"{TestHelpers.HivePath}/SAM");
        var desc = Helpers.GetDescriptionFromEnumValue(samOnDemand.HiveType);

        var en = Helpers.GetEnumValueFromDescription<HiveTypeEnum>(desc);

        Check.ThatCode(() =>
            {
                var enBad = Helpers.GetEnumValueFromDescription<int>("NotAnEnum");
            })
            .Throws<ArgumentException>();

        Check.That(desc).IsNotEmpty();
        Check.That(desc).Equals("SAM");
        Check.That(en).IsInstanceOf<HiveTypeEnum>();
        Check.That(en).Equals(HiveTypeEnum.Sam);
    }

    [Test]
    public void ShouldCatchNkRecordThatsTooSmallFromSlackSpace()
    {
        var usrclass = new RegistryHive($"{TestHelpers.HivePath}/ERZ_Win81_UsrClass.dat");
        usrclass.RecoverDeleted = true;
        usrclass.ParseHive();
    }

    [Test]
    public void ShouldCatchSlackRecordTooSmallToGetSignatureFrom()
    {
        var usrclass = new RegistryHive($"{TestHelpers.HivePath}/UsrClassJVM.dat");
        usrclass.RecoverDeleted = true;
        usrclass.ParseHive();
    }

    [Test]
    public void ShouldCatchVkRecordThatsTooSmallFromSlackSpace()
    {
        var usrclass = new RegistryHive($"{TestHelpers.HivePath}/NTUSER slack.DAT");
        usrclass.RecoverDeleted = true;
        usrclass.ParseHive();
    }

    [Test]
    public void ShouldFindAdbRecordWhileParsing()
    {
        var usrclass = new RegistryHive($"{TestHelpers.HivePath}/SYSTEM");
        usrclass.ParseHive();
    }

    [Test]
    public void ShouldFindDataNode()
    {
        var bcd = new RegistryHive($"{TestHelpers.HivePath}/BCD");
        bcd.FlushRecordListsAfterParse = false;
        bcd.RecoverDeleted = true;
        bcd.ParseHive();

        var dnraw = bcd.ReadBytesFromHive(0x0000000000001100, 8);
        var dn = new DataNode(dnraw, 0x0000000000000100);

        Check.That(dn).IsNotNull();
        Check.That(dn.ToString()).IsNotEmpty();
        Check.That(dn.Signature).IsEmpty();
    }

    [Test]
    public void ShouldFindDbRecord()
    {
        var system = new RegistryHive($"{TestHelpers.HivePath}/System");
        system.FlushRecordListsAfterParse = false;
        system.ParseHive();

        var record = system.ListRecords[0x78f20] as DbListRecord;

        Check.That(record).IsNotNull();
        if (record is not null)
        {
            Check.That(record.ToString()).IsNotEmpty();
            record.IsReferenced = true;

            Check.That(record.IsReferenced).IsTrue();
            Check.That(record.NumberOfEntries).IsEqualTo(2);
            Check.That(record.OffsetToOffsets).IsEqualTo(0x78F30);
        }
    }


    [Test]
    public void ShouldFindLfListRecord()
    {
        var bcd = new RegistryHive($"{TestHelpers.HivePath}/BCD");
        bcd.FlushRecordListsAfterParse = false;
        bcd.RecoverDeleted = true;
        bcd.ParseHive();

        var record = bcd.ListRecords[0xd0] as LxListRecord;

        Check.That(record).IsNotNull();
        if (record is not null)
        {
            Check.That(record.ToString()).IsNotEmpty();
        }
    }

    [Test]
    public void ShouldFindLhListRecord()
    {
        var drivers = new RegistryHive($"{TestHelpers.HivePath}/DRIVERS");
        drivers.FlushRecordListsAfterParse = false;
        drivers.RecoverDeleted = true;
        drivers.ParseHive();

        var record = drivers.ListRecords[0x270] as LxListRecord;

        Check.That(record).IsNotNull();
        if (record is not null)
        {
            Check.That(record.ToString()).IsNotEmpty();
        }
    }

    [Test]
    public void ShouldFindLiRecord()
    {
        var usrClass1 = new RegistryHive($"{TestHelpers.HivePath}/UsrClass 1.dat");
        usrClass1.RecoverDeleted = true;
        usrClass1.FlushRecordListsAfterParse = false;
        usrClass1.ParseHive();


        var record = usrClass1.ListRecords[0x000000000015f020] as LiListRecord;

        Check.That(record).IsNotNull();
        if (record is not null)
        {
            record.IsReferenced = true;
            Check.That(record.IsReferenced).IsTrue();
            Check.That(record.NumberOfEntries).IsEqualTo(696);
            Check.That(record.Size).IsEqualTo(0x00001630);
            Check.That(record.Offsets[0]).IsEqualTo(0x103078);
            Check.That(record.Offsets[1]).IsEqualTo(0x103EE0);
            Check.That(record.ToString()).IsNotEmpty();
        }
    }

    [Test]
    public void ShouldFindRiRecord()
    {
        var system = new RegistryHive($"{TestHelpers.HivePath}/System");
        system.FlushRecordListsAfterParse = false;
        system.ParseHive();

        var record = system.ListRecords[0x7141D0] as RiListRecord;

        Check.That(record).IsNotNull();
        if (record is not null)
        {
            record.IsReferenced = true;
            Check.That(record.IsReferenced).IsTrue();
            Check.That(record.NumberOfEntries).IsEqualTo(2);
            Check.That(record.Offsets[0]).IsEqualTo(0x717020);
            Check.That(record.Offsets[1]).IsEqualTo(0x72F020);
            Check.That(record.ToString()).IsNotEmpty();
        }
    }

    [Test]
    public void TestGetSidTypeFromSidString()
    {
        var sid = "S-1-5-5-111111";

        var desc = Helpers.GetSidTypeFromSidString(sid);

        Check.That(desc).IsInstanceOf<Helpers.SidTypeEnum>();

        sid = "S-1-2-0";

        desc = Helpers.GetSidTypeFromSidString(sid);

        Check.That(desc).IsEqualTo(Helpers.SidTypeEnum.Local);
    }

    [Test]
    public void VerifyHeaderInfo()
    {
        var sam = new RegistryHive($"{TestHelpers.HivePath}/SAM");
        sam.FlushRecordListsAfterParse = false;
        sam.ParseHive();

        Check.That(sam.Header).IsNotNull();
        if (sam.Header is not null)
        {
            Check.That(sam.Header.FileName).IsNotNull();
            Check.That(sam.Header.FileName).IsNotEmpty();
            Check.That(sam.Header.Length).IsStrictlyGreaterThan(0);
            Check.That(sam.Header.MajorVersion).IsStrictlyGreaterThan(0);
            Check.That(sam.Header.MinorVersion).IsStrictlyGreaterThan(0);
            Check.That(sam.Header.RootCellOffset).IsStrictlyGreaterThan(0);
            Check.That(sam.Header.CalculatedChecksum).Equals(sam.Header.CheckSum);
            Check.That(sam.Header.ValidateCheckSum()).Equals(true);
            Check.That(sam.Header.ToString()).IsNotEmpty();
        }
    }
}