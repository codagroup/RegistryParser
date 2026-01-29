#region Usings
using CODA.RegistryParser.Other;
#endregion

namespace CODA.RegistryParser;
public interface IRegistry
{
    #region Properties
    byte[] FileBytes { get; }

    HiveTypeEnum HiveType { get; }

    string HivePath { get; }

    RegistryHeader Header { get; set; }

    byte[] ReadBytesFromHive(long offset, int length);
    #endregion
}
