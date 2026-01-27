#region Usings
using NFluent;
using NUnit.Framework;
#endregion

namespace CODA.RegistryParser.Test;

[TestFixture]
public class TestRegistryBase
{
    [Test]
    public void BcdHiveShouldHaveBcdHiveType()
    {
        var r = new RegistryBase($"{TestHelpers.HivePath}/BCD");
        Check.That(HiveTypeEnum.Bcd).IsEqualTo(r.HiveType);
    }

    [Test]
    public void DriversHiveShouldHaveDriversHiveType()
    {
        var r = new RegistryBase($"{TestHelpers.HivePath}/Drivers");
        Check.That(HiveTypeEnum.Drivers).IsEqualTo(r.HiveType);
    }

    [Test]
    public void FileNameNotFoundShouldThrowFileNotFoundException()
    {
        Check.ThatCode(() => { new RegistryBase(@"/doesnotexist.reg"); })
            .Throws<FileNotFoundException>();
    }

    [Test]
    public void FileNameNotFoundShouldThrowNotSupportedException()
    {
        Check.ThatCode(() => { new RegistryBase(); }).Throws<NotSupportedException>();
    }

    [Test]
    public void HivePathShouldReflectWhatIsPassedIn()
    {
        var security = new RegistryHiveOnDemand($"{TestHelpers.HivePath}/SECURITY");

        Check.That(security.HivePath).IsEqualTo($"{TestHelpers.HivePath}/SECURITY");
    }

    [Test]
    public void InvalidRegistryHiveShouldThrowException()
    {
        Check.ThatCode(() => { new RegistryBase($"{TestHelpers.HivePath}/NotAHive"); }).Throws<Exception>();
    }


    [Test]
    public void NtuserHiveShouldHaveNtuserHiveType()
    {
        var r = new RegistryBase($"{TestHelpers.HivePath}/NTUSER.DAT");
        Check.That(HiveTypeEnum.NtUser).IsEqualTo(r.HiveType);
    }

    [Test]
    public void NullByteArrayShouldThrowEArgumentNullException()
    {
        byte[] nullBytes = {0x00};
        Check.ThatCode(() => { new RegistryBase(nullBytes, string.Empty); }).Throws<ArgumentException>();
    }

    [Test]
    public void NullFileNameShouldThrowEArgumentNullException()
    {
        string? nullFileName = null;
        Check.ThatCode(() => { new RegistryBase(nullFileName!); }).Throws<ArgumentNullException>();
    }

    [Test]
    public void OtherHiveShouldHaveOtherHiveType()
    {
        var r = new RegistryBase($"{TestHelpers.HivePath}/SAN(OTHER)");
        Check.That(HiveTypeEnum.Other).IsEqualTo(r.HiveType);
    }

    [Test]
    public void SamHiveShouldHaveSamHiveType()
    {
        var r = new RegistryBase($"{TestHelpers.HivePath}/SAM");
        Check.That(HiveTypeEnum.Sam).IsEqualTo(r.HiveType);
    }

    [Test]
    public void SecurityHiveShouldHaveSecurityHiveType()
    {
        var r = new RegistryBase($"{TestHelpers.HivePath}/Security");
        Check.That(HiveTypeEnum.Security).IsEqualTo(r.HiveType);
    }

    [Test]
    public void ShouldTakeByteArrayInConstructor()
    {
        var fileStream = new FileStream($"{TestHelpers.HivePath}/SAM", FileMode.Open, FileAccess.Read, FileShare.Read);
        var binaryReader = new BinaryReader(fileStream);

        binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

        var fileBytes = binaryReader.ReadBytes((int) binaryReader.BaseStream.Length);

        binaryReader.Close();
        fileStream.Close();

        var r = new RegistryBase(fileBytes, $"{TestHelpers.HivePath}/SAM");

        Check.That(r.Header).IsNotNull();
        Check.That(r.HivePath).IsEqualTo($"{TestHelpers.HivePath}/SAM");
        Check.That(r.HiveType).IsEqualTo(HiveTypeEnum.Sam);
    }

    [Test]
    public void ShouldThrowExceptionWhenNotRegistryHiveAndByteArray()
    {
        var fileStream = new FileStream($"{TestHelpers.HivePath}/NotAHive", FileMode.Open, FileAccess.Read, FileShare.Read);
        var binaryReader = new BinaryReader(fileStream);

        binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

        var fileBytes = binaryReader.ReadBytes((int) binaryReader.BaseStream.Length);

        binaryReader.Close();
        fileStream.Close();

        Check.ThatCode(() =>
            {
                var rb = new RegistryBase(fileBytes, $"{TestHelpers.HivePath}/NotAHive");
            })
            .Throws<ArgumentException>();
    }

    [Test]
    public void SoftwareHiveShouldHaveSoftwareHiveType()
    {
        var r = new RegistryBase($"{TestHelpers.HivePath}/software");
        Check.That(HiveTypeEnum.Software).IsEqualTo(r.HiveType);
    }

    [Test]
    public void SystemHiveShouldHaveSystemHiveType()
    {
        var r = new RegistryBase($"{TestHelpers.HivePath}/system");
        Check.That(HiveTypeEnum.System).IsEqualTo(r.HiveType);
    }

    [Test]
    public void UsrclassHiveShouldHaveUsrclassHiveType()
    {
        var r = new RegistryBase($"{TestHelpers.HivePath}/UsrClass 1.dat");
        Check.That(HiveTypeEnum.UsrClass).IsEqualTo(r.HiveType);
    }
}