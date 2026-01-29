#region Usings
using System.Text;
using CODA.RegistryParser.Other;
using Serilog;
using static CODA.RegistryParser.Other.Helpers;
#endregion

namespace CODA.RegistryParser;
public class RegistryBase : IRegistry
{
    #region Constructors
    public RegistryBase()
    {
        throw new NotSupportedException("Call the other constructor and pass in the path to the Registry hive!");
    }

    #pragma warning disable CS8618 // Non-nullable properties Header & Version are set by the Initialize() function.
    public RegistryBase(byte[] rawBytes, string hivePath)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        FileBytes = rawBytes;
        HivePath = "None";


        if (!HasValidSignature())
        {
            Log.Error("Data in byte array is not a Registry hive (bad signature)");

            throw new ArgumentException("Data in byte array is not a Registry hive (bad signature)");
        }

        HivePath = hivePath;

        Initialize();
    }
    #pragma warning restore CS8618
    #pragma warning disable CS8618 // Non-nullable properties Header & Version are set by the Initialize() function.
    public RegistryBase(string hivePath)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        if (hivePath == null) throw new ArgumentNullException("hivePath cannot be null");

        if (!File.Exists(hivePath))
        {
            var fullPath = Path.GetFullPath(hivePath);
            throw new FileNotFoundException($"The specified file {fullPath} was not found.", fullPath);
        }

        var fileStream = new FileStream(hivePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var binaryReader = new BinaryReader(fileStream);

        binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

        FileBytes = binaryReader.ReadBytes((int) binaryReader.BaseStream.Length);

        binaryReader.Close();
        fileStream.Close();


        if (!HasValidSignature())
        {
            Log.Error("{HivePath} is not a Registry hive (bad signature)", hivePath);

            throw new Exception($"{hivePath} is not a Registry hive (bad signature)");
        }

        HivePath = hivePath;

        Initialize();
    }
    #pragma warning restore CS8618
    #endregion
    #region Properties
    public long TotalBytesRead { get; internal set; }
    public string Version { get; private set; }

    public byte[] FileBytes { get; internal set; }

    public HiveTypeEnum HiveType { get; private set; }

    public string HivePath { get; }

    public RegistryHeader Header { get; set; }
    #endregion
    #region Functions
    public byte[] ReadBytesFromHive(long offset, int length)
    {
        var readLength = Math.Abs(length);

        var remaining = FileBytes.Length - offset;

        if (remaining <= 0) return new byte[0];

        if (readLength > remaining) readLength = (int) remaining;

        var r = new ArraySegment<byte>(FileBytes, (int) offset, readLength);

        return r.ToArray();
    }

    internal void Initialize()
    {
        var header = ReadBytesFromHive(0, 4096);

        Header = new RegistryHeader(header);

        var fileNameSegs = Header.FileName.Split('\\');

        var fNameBase = fileNameSegs.Last().ToLowerInvariant();
        
        Log.Debug("Got hive header. Embedded file name {FileName}. Base Name {Base}", Header.FileName,fNameBase);

        switch (fNameBase)
        {
            case "ntuser.dat":
                HiveType = HiveTypeEnum.NtUser;
                break;
            case "sam":
                HiveType = HiveTypeEnum.Sam;
                break;
            case "security":
                HiveType = HiveTypeEnum.Security;
                break;
            case "software":
                HiveType = HiveTypeEnum.Software;
                break;
            case "system":
                HiveType = HiveTypeEnum.System;
                break;
            case "drivers":
                HiveType = HiveTypeEnum.Drivers;
                break;
            case "usrclass.dat":
                HiveType = HiveTypeEnum.UsrClass;
                break;
            case "components":
                HiveType = HiveTypeEnum.Components;
                break;
            case "bcd":
                HiveType = HiveTypeEnum.Bcd;
                break;
            case "amcache.hve":
            case "amcache.hve.tmp":
                HiveType = HiveTypeEnum.Amcache;
                break;
            case "syscache.hve":
                HiveType = HiveTypeEnum.Syscache;
                break;
            case "elam": 
                HiveType = HiveTypeEnum.Elam;
                break;
            case "default": 
                HiveType = HiveTypeEnum.Default;
                break;
            case "Vsmidk": 
                HiveType = HiveTypeEnum.Vsmidk;
                break;
            case "BcdTemplate":
                HiveType = HiveTypeEnum.BcdTemplate;
                break;
            case "bbi": 
                HiveType = HiveTypeEnum.Bbi;
                break;
            case "userdiff": 
                HiveType = HiveTypeEnum.Userdiff;
                break;
            case "user.dat": 
                HiveType = HiveTypeEnum.User;
                break;
            case "userclasses.dat": 
                HiveType = HiveTypeEnum.UserClasses;
                break;
            case "settings.dat": 
                HiveType = HiveTypeEnum.settings;
                break;
            case "registry.dat": 
                HiveType = HiveTypeEnum.Registry;
                break;
            default:
                HiveType = HiveTypeEnum.Other;
                break;
        }

        Version = $"{Header.MajorVersion}.{Header.MinorVersion}";
    }

    public bool HasValidSignature()
    {
        var sig = BitConverter.ToInt32(FileBytes, 0);

        return sig.Equals(RegfSignature);
    }
    #endregion
}
