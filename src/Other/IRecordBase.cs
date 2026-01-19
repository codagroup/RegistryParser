namespace CODA.RegistryParser.Other;

public interface IRecordBase
{
    #region Properties
    /// <summary>
    ///     The offset in the registry hive file to a record
    /// </summary>
    long AbsoluteOffset { get; }

    string Signature { get; }
    #endregion
}